namespace Authorization.Consumer.Infrastructure.Interface.Kafka
{
    public interface IKafkaListener
    {
        string ConsumerGroupId { get; set; }
        IEnumerable<string> Topics { get; set; }
    }
}
