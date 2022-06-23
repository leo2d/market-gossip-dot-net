using MarketGossip.Shared.Events;
using MarketGossip.Shared.IntegrationBus.Contracts;

namespace MarketGossip.ChatApp.Application.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder StartListeningToIntegrationEvents(this IApplicationBuilder app,
        IConfiguration configuration)
    {
        var integrationBusConsumer = app.ApplicationServices.GetRequiredService<IIntegrationBusConsumer>();

        integrationBusConsumer.StartListening<StockQuoteProcessed>(
            configuration["EventQueues:StockQuoteProcessedQueue"]);

        return app;
    }
}