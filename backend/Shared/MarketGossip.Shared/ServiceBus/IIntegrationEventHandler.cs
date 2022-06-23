using MarketGossip.Shared.Events;

namespace MarketGossip.Shared.ServiceBus;

public interface IIntegrationEventHandler<in TEvent>
    where TEvent : IntegrationEvent
{
    Task Handle(TEvent @event);
}