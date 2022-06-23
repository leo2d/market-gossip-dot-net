using MarketGossip.Shared.Events;
using MarketGossip.Shared.IntegrationBus.Contracts;

namespace MarketGossip.IntegrationWorkers.EventHandling;

public class StockQuoteRequestedEventHandler : IIntegrationEventHandler<StockQuoteRequested>
{
    private readonly IIntegrationBusPublisher _integrationBusPublisher;
    private readonly IConfiguration _configuration;
    private readonly IStooqService _stooqService;

    public StockQuoteRequestedEventHandler(
        IConfiguration configuration,
        IIntegrationBusPublisher integrationBusPublisher,
        IStooqService stooqService)
    {
        _configuration = configuration;
        _integrationBusPublisher = integrationBusPublisher;
        _stooqService = stooqService;
    }

    public async Task Handle(StockQuoteRequested @event)
    {
        var result = await _stooqService.GetStockInfoAsCsv(@event.Symbol);

        var eventResult = new StockQuoteProcessed
        {
            Symbol = @event.Symbol,
            QuoteValue = 9999
        };

        var queue = _configuration["EventQueues:StockQuoteProcessedQueue"];

        await _integrationBusPublisher.PublishAsync(queue, eventResult);
    }
}