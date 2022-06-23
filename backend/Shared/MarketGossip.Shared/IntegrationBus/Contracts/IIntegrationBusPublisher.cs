using MarketGossip.Shared.Events;

namespace MarketGossip.Shared.IntegrationBus.Contracts;

public interface IIntegrationBusPublisher
{
    Task PublishAsync<TEvent>(string queue, TEvent @event) where TEvent : IntegrationEvent;
}