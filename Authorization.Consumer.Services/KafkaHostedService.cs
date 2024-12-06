using Authorization.Consumer.Infrastructure.Interface.Kafka;
using Kafka.Public;
using Kafka.Public.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using WLS.KafkaProcessor.Services.Interfaces;
using WLS.Log.LoggerTransactionPattern;

namespace Authorization.Consumer.Services
{
    public class KafkaHostedService : IHostedService
    {
        private readonly object _lockObject = new();
        private readonly ILogger<KafkaHostedService> _logger;
        private readonly Dictionary<string, ClusterClient> _cluster;
        private readonly IKafkaConfig _kafkaConfig;
        private readonly IMappedServices _mappedServices;
        private readonly IServiceProvider _provider;
        private readonly IKafkaConsumer _consumer;
        private readonly ILoggerStateFactory _loggerStateFactory;
        private readonly string assemblyName;
        public KafkaHostedService(IKafkaConfig kafkaConfig, IMappedServices mappedServices, IServiceProvider provider,
            IKafkaConsumer consumer, ILogger<KafkaHostedService> logger, ILoggerStateFactory loggerStateFactory)
        {
            _logger = logger;
            _kafkaConfig = kafkaConfig;
            //_cluster = new ClusterClient(new Configuration
            //{
            //    Seeds = _kafkaConfig.Host
            //}, new ConsoleLogger());
            //_mappedServices = mappedServices;
            //_provider = provider;
            //_provider.CreateScope();
            //_consumer = consumer;

            _cluster = new Dictionary<string, ClusterClient>();

            _kafkaConfig.Listeners.ToList().ForEach(listener =>
            {
                foreach (var topic in listener.Topics)
                {
                    _cluster.Add(topic, new ClusterClient(new Configuration
                    {
                        Seeds = _kafkaConfig.Host
                    }, new ConsoleLogger())
                    );
                }
            });

            provider.CreateScope();

            _mappedServices = mappedServices;
            _consumer = consumer;
            _loggerStateFactory = loggerStateFactory;
            assemblyName = Assembly.GetExecutingAssembly().GetName().Name.ToString();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //_kafkaConfig.Listeners.ToList().ForEach(listener =>
            //{
            //    _cluster.Subscribe(listener.ConsumerGroupId, listener.Topics,
            //    new ConsumerGroupConfiguration
            //    {
            //        AutoCommitEveryMs = 5000
            //    });
            //    _cluster.MessageReceived += _cluster_MessageReceived;
            //});
            //return Task.CompletedTask;

            _logger.LogInformation($"{assemblyName} - KafkaHostedService - StartAsync() invoked at {DateTime.Now:G}");

            if (Monitor.TryEnter(_lockObject))
            {
                Guid transactionID = Guid.NewGuid();
                try
                {
                    using (_logger.BeginScope(_loggerStateFactory.Create(transactionID.ToString())))
                    {
                        _kafkaConfig.Listeners.ToList().ForEach(listener =>
                        {
                            foreach (KeyValuePair<string, ClusterClient> cluster in _cluster)
                            {
                                cluster.Value.Subscribe(listener.ConsumerGroupId + "-" + cluster.Key.Replace("wly.glb.pl.", ""), cluster.Key.Split(""),
                                new ConsumerGroupConfiguration
                                {
                                    AutoCommitEveryMs = 5000
                                });
                                cluster.Value.MessageReceived += ClusterMessageReceived;
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{assemblyName} - KafkaHostedService - StartAsync() threw Exception{Environment.NewLine}{ex}");
                }
            }

            return Task.CompletedTask;
        }

        private void ClusterMessageReceived(RawKafkaRecord record)
        {
            try
            {
                _consumer.Consume(record, _mappedServices);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{assemblyName} - KafkaHostedService - ClusterMessageReceived() threw Exception{Environment.NewLine}{ex}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{assemblyName} - KafkaHostedService - StopAsync() invoked at {DateTime.Now:G}");

            foreach (KeyValuePair<string, ClusterClient> cluster in _cluster)
            {
                cluster.Value.Dispose();
            }

            return Task.CompletedTask;
        }
    }
}
