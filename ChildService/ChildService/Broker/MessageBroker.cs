using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace ChildService.Broker
{
    public class MessageBroker : IMessageBroker
    {
        private readonly IConfiguration _configuration;

        public MessageBroker(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConnection CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["MessageBroker:HostName"],
                Port = int.Parse(_configuration["MessageBroker:Port"]),
                UserName = _configuration["MessageBroker:Username"],
                Password = _configuration["MessageBroker:Password"]
            };

            return factory.CreateConnection();
        }

        public void Publish<T>(T message)
        {
            using var connection = CreateConnection();
            using var channel = connection.CreateModel();

            var queueName = _configuration["MessageBroker:QueueName"];
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
        }
    }
}
