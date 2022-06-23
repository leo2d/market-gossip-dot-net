using System.Text;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.Extensions;
using MarketGossip.Shared.IntegrationBus.Contracts;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace MarketGossip.Shared.IntegrationBus;

public class IntegrationBusPublisher : IIntegrationBusPublisher
{
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly ILogger<IntegrationBusPublisher> _logger;

    public IntegrationBusPublisher(IRabbitMqClient rabbitMqClient,
        ILogger<IntegrationBusPublisher> logger)
    {
        _rabbitMqClient = rabbitMqClient;
        _logger = logger;
    }

    public Task PublishAsync<TEvent>(string queue, TEvent @event) where TEvent : IntegrationEvent
    {
        var channel = _rabbitMqClient.GetOrCreateChannelForEvent<TEvent>(queue);

        var message = @event.ToJson();

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "",
            routingKey: queue,
            basicProperties: null,
            body);

        return Task.CompletedTask;
    }
}