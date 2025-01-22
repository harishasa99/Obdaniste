using RabbitMQ.Client;

namespace ChildService.Broker
{
    public interface IMessageBroker
    {
        void Publish<T>(T message);
        IConnection CreateConnection();
    }
}
