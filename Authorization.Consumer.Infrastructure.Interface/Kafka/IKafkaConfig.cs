namespace Authorization.Consumer.Infrastructure.Interface.Kafka
{
    public interface IKafkaConfig
    {
        string Host { get; set; }
        IEnumerable<IKafkaListener> Listeners { get; set; }
    }
}
