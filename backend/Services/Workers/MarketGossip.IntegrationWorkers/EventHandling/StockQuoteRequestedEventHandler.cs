using MarketGossip.IntegrationWorkers.Options;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.IntegrationBus.Contracts;
using Microsoft.Extensions.Options;

namespace MarketGossip.IntegrationWorkers.EventHandling;

public class StockQuoteRequestedEventHandler : IIntegrationEventHandler<StockQuoteRequested>
{
    private readonly IIntegrationBusPublisher _integrationBusPublisher;
    private readonly IStooqService _stooqService;
    private readonly EventQueuesConfig _queuesConfig;

    public StockQuoteRequestedEventHandler(
        IIntegrationBusPublisher integrationBusPublisher,
        IStooqService stooqService,
        IOptions<EventQueuesConfig> queuesConfigOptions)
    {
        _queuesConfig = queuesConfigOptions?.Value ?? throw new ArgumentNullException(nameof(queuesConfigOptions));

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

        await _integrationBusPublisher.PublishAsync(_queuesConfig.StockQuoteProcessed, eventResult);
    }
}