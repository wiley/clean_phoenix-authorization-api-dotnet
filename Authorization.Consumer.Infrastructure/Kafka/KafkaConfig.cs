using Authorization.Consumer.Infrastructure.Interface.Kafka;

namespace Authorization.Consumer.Infrastructure.Kafka
{
    public class KafkaConfig : IKafkaConfig
    {
        public string Host { get; set; }
        public IEnumerable<IKafkaListener> Listeners { get; set; }
    }
}
