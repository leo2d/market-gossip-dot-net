using MarketGossip.Shared.Events;

namespace MarketGossip.Shared.IntegrationBus.Contracts;

public interface IIntegrationBusConsumer
{
    void StartListening<TEvent>(string queue) where TEvent : IntegrationEvent;
}