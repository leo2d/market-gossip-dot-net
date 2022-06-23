using MarketGossip.Shared.Events;
using MarketGossip.Shared.ServiceBus;

namespace MarketGossip.IntegrationWorkers.EventHandling;

public interface blabla : IIntegrationEventHandler<StockQuoteRequested>
{
}

public class StockQuoteRequestedEventHandler : IIntegrationEventHandler<StockQuoteRequested>
{
    private readonly IntegrationEventHandler _integrationBus;
    private readonly IConfiguration _configuration;

    public StockQuoteRequestedEventHandler(
        // IntegrationEventHandler integrationBus,
        IConfiguration configuration)
    {
        // _integrationBus = integrationBus;
        _configuration = configuration;
    }

    public async Task Handle(StockQuoteRequested @event)
    {
        var eventResult = new StockQuoteProcessed
        {
            Symbol = "test from worker",
            QuoteValue = 9999
        };

        var queue = _configuration["EventQueues:StockQuoteProcessedQueue"];

        // await _integrationBus.PublishAsync(queue, eventResult);
    }
}