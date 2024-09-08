using CodeChamp.RabbitMQ;
using DMS.API.Hubs;
using DMS.Core.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace DMS.API.Service
{
    public class LogConsumerService : BackgroundService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly MqConsumer _consumer;

        public LogConsumerService(IHubContext<NotificationHub> hubContext, MqConsumer consumer)
        {
            _hubContext = hubContext;
            _consumer = consumer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "user", Password = "mypass", VirtualHost = "/" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "actionLogsQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            //var consumer = new EventingBasicConsumer(channel);
            //consumer.Received += async (model, ea) =>
            //{
            //    var body = ea.Body.ToArray();
            //    var message = Encoding.UTF8.GetString(body);
            //    await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
            //};
            _consumer.AddListener("actionLogsQueue", async (_, args) =>
            {
                var body = args.Body.ToArray();
                var postJson = Encoding.UTF8.GetString(body);
                var actionLog = JsonSerializer.Deserialize<ActionLogEvent>(postJson);
                await _hubContext.Clients.Group("Admins").SendAsync("ReceiveNotification", actionLog);

            });
           // channel.BasicConsume(queue: "actionLogsQueue", autoAck: true, consumer: _consumer);

            return Task.CompletedTask;
        }
    }
}
