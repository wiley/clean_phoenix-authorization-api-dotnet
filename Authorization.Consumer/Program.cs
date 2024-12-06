using Authorization.Consumer.Domain;
using Authorization.Consumer.Infrastructure.Interface.Kafka;
using Authorization.Consumer.Infrastructure.Kafka;
using Authorization.Consumer.Services;
using Authorization.Consumer.Services.Kafka.Executors;
using Authorization.Domain.Company.OrganizationRole;
using Authorization.Domain.User;
using Authorization.Infrastructure.Interface.Mongo;
using Authorization.Infrastructure.Mongo;
using Authorization.Services;
using Authorization.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;
using WLS.KafkaProcessor.Domain;
using WLS.KafkaProcessor.Services;
using WLS.KafkaProcessor.Services.Executors.Interfaces;
using WLS.KafkaProcessor.Services.Interfaces;
using WLS.Log.LoggerTransactionPattern;

namespace Authorization.Consumer
{
    public class Program
    {
        protected Program()
        { }
        private static Dictionary<string, MessageTypeServiceMap> mappedServices = new Dictionary<string, MessageTypeServiceMap>();
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            string KafkaHost = Environment.GetEnvironmentVariable("KAFKA_HOST");
            string KafkaConsumerGroup = $"{Environment.GetEnvironmentVariable("KAFKA_CONSUMER_GROUP")}_{Guid.NewGuid()}";
            string KafkaTopic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            var connString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
            var dbName = Environment.GetEnvironmentVariable("MONGO_DB_NAME");

            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, collection) => 
                {
                    var listerners = new List<IKafkaListener>();
                    listerners.Add(new KafkaListener
                    {
                        ConsumerGroupId = KafkaConsumerGroup,
                        Topics = KafkaTopic.Split(",")
                    });
                    collection.AddSingleton<IKafkaConfig>(kc =>
                     new KafkaConfig() { Host = KafkaHost, Listeners = listerners }
                    );

                    var dbSettings = new MongoDBSettings { ConnectionString = connString, DatabaseName = dbName };
                    collection.AddSingleton<IMongoDBSettings>(_ => dbSettings);
                    collection.AddSingleton<IMongoClient>(_ => new MongoClient(connString));
                    collection.AddSingleton<IConnectionThrottlingPipeline, ConnectionThrottlingPipeline>();
                    collection.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
                    collection.AddScoped<IUserService, UserService>();
                    collection.AddScoped<ICompanyService, CompanyService>();
                    collection.AddScoped<IEntitlementService, EntitlementService>();
                    collection.AddHttpClient();

                    collection.AddScoped<IKafkaConsumer, KafkaConsumer>();
                    collection.AddScoped<ILoggerStateFactory, LoggerStateFactory>();
                    AddMappedServices<AccountUserRole, AccountUserRoleMessageExecutor>(collection);
                    AddMappedServices<OrganizationUserRole, OrganizationUserRoleMessageExecutor>(collection);
                    AddMappedServices<UserModel, UserMessageExecutor>(collection);
                    AddMappedServices<Entitlement, EntitlementMessageExecutor>(collection);
                    AddMappedServices<GroupV4, GroupsMessageExecutor>(collection);

                    collection.AddSingleton<IMappedServices>(mp => new MappedServices() { Services = mappedServices });
                    collection.AddHostedService<KafkaHostedService>();
                }).ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                });
        }

        private static void AddMappedServices<TModel,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        TExecutor>(IServiceCollection collection)
            where TModel : class
            where TExecutor : class, IKafkaExecutor<TModel>
        {
            collection.AddScoped<IKafkaExecutor<TModel>, TExecutor>();
            mappedServices.Add(typeof(TModel).Name, new MessageTypeServiceMap
            {
                MessageType = typeof(TModel),
                ServiceType = typeof(IKafkaExecutor<TModel>)
            });
        }
    }
}