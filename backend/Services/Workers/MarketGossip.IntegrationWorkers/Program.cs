using MarketGossip.IntegrationWorkers;
using MarketGossip.IntegrationWorkers.EventHandling;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.Extensions;
using MarketGossip.Shared.ServiceBus;

IConfiguration? configuration = null;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builderContext, services) =>
    {
        configuration = builderContext.Configuration;
        services.AddTransient<IIntegrationEventHandler<StockQuoteRequested>, StockQuoteRequestedEventHandler>();
        services.AddTransient<IStooqService, StooqService>();

        services.AddHostedService<Worker>();

        services.SetupIntegrationBus(configuration);
    })
    .Build();

// var eventHandler = host.Services.GetRequiredService<IntegrationEventHandler>();
// eventHandler.StartListening<StockQuoteRequested>(configuration!["EventQueues:StockQuoteRequestedQueue"]);

await host.RunAsync();