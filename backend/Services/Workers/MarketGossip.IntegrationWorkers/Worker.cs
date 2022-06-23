using MarketGossip.IntegrationWorkers.Options;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.IntegrationBus.Contracts;
using Microsoft.Extensions.Options;

namespace MarketGossip.IntegrationWorkers;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IIntegrationBusConsumer _integrationBusConsumer;
    private readonly EventQueuesConfig _queuesConfig;

    public Worker(ILogger<Worker> logger,
        IIntegrationBusConsumer integrationBusConsumer,
        IOptions<EventQueuesConfig> queuesConfigOptions)
    {
        _queuesConfig = queuesConfigOptions?.Value ?? throw new ArgumentNullException(nameof(queuesConfigOptions));

        _logger = logger;
        _integrationBusConsumer = integrationBusConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _integrationBusConsumer.StartListening<StockQuoteRequested>(_queuesConfig.StockQuoteRequested);

            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

            await Task.Delay(1000, stoppingToken);
        }
    }
}