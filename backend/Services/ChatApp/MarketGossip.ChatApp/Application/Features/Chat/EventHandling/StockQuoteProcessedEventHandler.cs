using MarketGossip.Shared.Events;
using MarketGossip.Shared.IntegrationBus.Contracts;

namespace MarketGossip.ChatApp.Application.Features.Chat.EventHandling;

public class StockQuoteProcessedEventHandler : IIntegrationEventHandler<StockQuoteProcessed>
{
    private readonly ILogger<StockQuoteProcessedEventHandler> _logger;

    public StockQuoteProcessedEventHandler(ILogger<StockQuoteProcessedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(StockQuoteProcessed @event)
    {
        _logger.LogInformation("Stock quote processed {StockCode}", @event.Symbol);

        return Task.CompletedTask;
    }
}