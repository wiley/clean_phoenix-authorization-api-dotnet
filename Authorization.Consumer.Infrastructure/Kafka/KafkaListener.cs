using Authorization.Consumer.Infrastructure.Interface.Kafka;

namespace Authorization.Consumer.Infrastructure.Kafka
{
    public class KafkaListener : IKafkaListener
    {
        public string ConsumerGroupId { get; set; }
        public IEnumerable<string> Topics { get; set; }
    }
}
