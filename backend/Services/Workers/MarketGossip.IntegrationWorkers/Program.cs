using MarketGossip.IntegrationWorkers;
using MarketGossip.IntegrationWorkers.EventHandling;
using MarketGossip.IntegrationWorkers.Options;
using MarketGossip.IntegrationWorkers.Services;
using MarketGossip.IntegrationWorkers.Services.Contracts;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.IntegrationBus.Contracts;
using MarketGossip.Shared.IntegrationBus.Extensions;
using Polly;
using Polly.Extensions.Http;

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builderContext, services) =>
    {
        //ATENTION: always add the workers first, then the rest
        services.AddHostedService<Worker>();

        var configuration = builderContext.Configuration;

        services.Configure<EventQueuesConfig>(configuration.GetSection(EventQueuesConfig.Section));

        services.AddTransient<IIntegrationEventHandler<StockQuoteRequested>, StockQuoteRequestedEventHandler>();

        services.AddHttpClient<IStooqService, StooqService>()
            .AddPolicyHandler(GetRetryPolicy());

        services.AddRabbitMqClient(configuration)
            .AddIntegrationBusConsumer()
            .AddIntegrationBusPublisher();
    })
    .Build();

await host.RunAsync();