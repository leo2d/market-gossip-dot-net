using MarketGossip.ChatApp.Application.Options;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.IntegrationBus.Contracts;
using Microsoft.Extensions.Options;

namespace MarketGossip.ChatApp.Application.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder StartListeningToIntegrationEvents(this IApplicationBuilder app,
        IConfiguration configuration)
    {
        var queuesConfigOptions = app.ApplicationServices.GetRequiredService<IOptions<EventQueuesConfig>>();

        var queueConfig = queuesConfigOptions?.Value ??
                          throw new Exception("EventQueuesConfig is null. Call Services.SetupConfigurations first");

        var integrationBusConsumer = app.ApplicationServices.GetRequiredService<IIntegrationBusConsumer>();

        integrationBusConsumer.StartListening<StockQuoteProcessed>(queueConfig.StockQuoteProcessed);

        return app;
    }
}