using RabbitMQ.Client;

namespace MarketGossip.Shared.IntegrationBus.Contracts;

public interface IRabbitMqClient
{
    IModel GetOrCreateChannelForEvent<TEvent>(string? queue = null);
}