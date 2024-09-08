using DMS.Core.Entities;
using DMS.Services.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace DMS.Services.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        public void SendMessage(ActionLogEvent logEntry)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "user", Password = "mypass", VirtualHost = "/" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "actionLogsQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            var message = JsonSerializer.Serialize(logEntry);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "actionLogsQueue", basicProperties: null, body: body);
        }
    }
}
