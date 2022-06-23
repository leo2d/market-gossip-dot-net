using MarketGossip.IntegrationWorkers;
using MarketGossip.IntegrationWorkers.EventHandling;
using MarketGossip.IntegrationWorkers.Options;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.IntegrationBus.Contracts;
using MarketGossip.Shared.IntegrationBus.Extensions;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builderContext, services) =>
    {
        //ATENTION: always add the workers first, then the rest
        services.AddHostedService<Worker>();

        var configuration = builderContext.Configuration;

        services.Configure<EventQueuesConfig>(configuration.GetSection(EventQueuesConfig.Section));

        services.AddTransient<IIntegrationEventHandler<StockQuoteRequested>, StockQuoteRequestedEventHandler>();
        services.AddTransient<IStooqService, StooqService>();

        services.AddRabbitMqClient(configuration)
            .AddIntegrationBusConsumer()
            .AddIntegrationBusPublisher();
    })
    .Build();

await host.RunAsync();