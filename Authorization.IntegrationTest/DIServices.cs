using Authorization.Infrastructure.Interface.Mongo;
using Authorization.Infrastructure.Mongo;
using Authorization.Services.Interfaces;
using Authorization.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Authorization.IntegrationTest
{
    public class DIServices
    {
        public ServiceProvider GenerateDependencyInjection()
        {
            var services = new ServiceCollection();
            RegisterServices(services);

            return services
                .AddLogging()
                .BuildServiceProvider();
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IMongoHealthCheckService, MongoHealthCheckService>();
            services.AddScoped(typeof(IPaginationService<>), typeof(PaginationService<>));
            RegisterRepositories(services);
        }

        private void RegisterRepositories(IServiceCollection services)
        {
            var cfg = GetConfiguraiton();
            var connString = cfg.MongoConnectionString;
            var dbName = cfg.MongoDBName;

            services.AddSingleton<IMongoDBSettings>(_ =>
                new MongoDBSettings()
                {
                    ConnectionString = connString,
                    DatabaseName = dbName
                });
            services.AddSingleton<IMongoClient>(_ => new MongoClient(connString));
            services.AddScoped<IConnectionThrottlingPipeline, ConnectionThrottlingPipeline>();
            services.AddScoped<IMongoTestConnection, MongoTestConnection>();
            services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        }

        private Config GetConfiguraiton()
        {
            var configObj = new Config();
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            config.GetSection("Config").Bind(configObj);
            return configObj;
        }
    }
}
