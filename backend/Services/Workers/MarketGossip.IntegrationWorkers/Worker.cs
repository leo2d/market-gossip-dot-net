using MarketGossip.IntegrationWorkers.EventHandling;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.ServiceBus;

namespace MarketGossip.IntegrationWorkers;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IntegrationEventHandler _integrationEventHandler;

    IServiceProvider _serviceProvider;

    public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger,
        IntegrationEventHandler integrationEventHandler)
    {
        _logger = logger;
        _integrationEventHandler = integrationEventHandler;

        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _integrationEventHandler.StartListening<StockQuoteRequested>("stock-quote-requested");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}