using System.Text;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace MarketGossip.Shared.ServiceBus;

public interface IIntegrationBus
{
    Task PublishAsync<TEvent>(string queue, TEvent @event) where TEvent : IntegrationEvent;
}

public class IntegrationBus : IIntegrationBus
{
    private readonly ILogger<IntegrationBus> _logger;
    private readonly IConfiguration _configuration;

    public IntegrationBus(IConfiguration configuration,
        ILogger<IntegrationBus> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task PublishAsync<TEvent>(string queue, TEvent @event) where TEvent : IntegrationEvent
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
