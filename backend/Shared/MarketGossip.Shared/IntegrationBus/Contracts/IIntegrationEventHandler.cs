using MarketGossip.Shared.Events;

namespace MarketGossip.Shared.IntegrationBus.Contracts;

public interface IIntegrationEventHandler<in TEvent>
    where TEvent : IntegrationEvent
{
    Task Handle(TEvent @event);
}