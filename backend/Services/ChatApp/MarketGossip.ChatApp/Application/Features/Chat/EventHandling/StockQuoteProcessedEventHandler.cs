using MarketGossip.Shared.Events;
using MarketGossip.Shared.ServiceBus;

namespace MarketGossip.ChatApp.Application.Features.Chat.EventHandling;

public class StockQuoteProcessedEventHandler : IIntegrationEventHandler<StockQuoteProcessed>
{
    public Task Handle(StockQuoteProcessed @event)
    {
        throw new NotImplementedException();
    }
}