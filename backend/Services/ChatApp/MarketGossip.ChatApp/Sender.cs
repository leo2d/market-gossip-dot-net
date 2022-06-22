using System.Text;
using MarketGossip.Shared.Extensions;
using RabbitMQ.Client;

namespace MarketGossip.ChatApp;

public interface IEventSender
{
    Task SendAsync(string queue, object @event);
}

public class EventSender : IEventSender
{
    private readonly IConfiguration _configuration;

    public EventSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task SendAsync(string queue, object @event)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:HostName"],
            Port = int.Parse(_configuration["RabbitMq:Port"]),
            UserName = _configuration["RabbitMq:UserName"],
            Password = _configuration["RabbitMq:Password"],
        };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var message = @event.ToJson();

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                routingKey: queue,
                basicProperties: null,
                body: body);
        }

        return Task.CompletedTask;
    }
}