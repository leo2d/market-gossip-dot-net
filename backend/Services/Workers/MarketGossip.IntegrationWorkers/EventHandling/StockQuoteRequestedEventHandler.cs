using MarketGossip.IntegrationWorkers.Dtos;
using MarketGossip.IntegrationWorkers.Options;
using MarketGossip.IntegrationWorkers.Services.Contracts;
using MarketGossip.Shared.Dtos.Result;
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
        IStooqService stooqService,
        IIntegrationBusPublisher integrationBusPublisher,
        IOptions<EventQueuesConfig> queuesConfigOptions)
    {
        _queuesConfig = queuesConfigOptions?.Value ?? throw new ArgumentNullException(nameof(queuesConfigOptions));

        _stooqService = stooqService;
        _integrationBusPublisher = integrationBusPublisher;
    }

    public async Task Handle(StockQuoteRequested @event)
    {
        var result = await _stooqService.GetStockInfo(@event.Symbol);

        var eventResult = CreateResponseMessage(@event, result);

        await _integrationBusPublisher.PublishAsync(_queuesConfig.StockQuoteProcessed, eventResult);
    }

    private static StockQuoteProcessed CreateResponseMessage(StockQuoteRequested @event, Result<StockInfo> result)
    {
        if (result.IsFailure)
        {
            return new StockQuoteProcessed
            {
                Symbol = @event.Symbol,
                Error = result.Error,
                Success = false,
                StockInfo = null,
            };
        }

        return new StockQuoteProcessed
        {
            Symbol = @event.Symbol,
            Error = null,
            Success = true,
            StockInfo = new StockQuoteProcessed.StockData
            {
                Symbol = result.Value.Symbol,
                Open = result.Value.Open,
                Close = result.Value.Close,
                Low = result.Value.Low,
                High = result.Value.High,
                Volume = result.Value.Volume,
                Date = result.Value.Date,
                Time = result.Value.Time,
            }
        };
    }
}