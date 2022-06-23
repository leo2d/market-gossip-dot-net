using MarketGossip.Shared.Events;
using MarketGossip.Shared.IntegrationBus.Contracts;

namespace MarketGossip.IntegrationWorkers;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IIntegrationBusConsumer _integrationBusConsumer;

    public Worker(ILogger<Worker> logger, IIntegrationBusConsumer integrationBusConsumer)
    {
        _logger = logger;
        _integrationBusConsumer = integrationBusConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _integrationBusConsumer.StartListening<StockQuoteRequested>("stock-quote-requested");

            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

            await Task.Delay(1000, stoppingToken);
        }
    }
}