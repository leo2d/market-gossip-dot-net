using MarketGossip.Shared.IntegrationBus.Contracts;
using MarketGossip.Shared.IntegrationBus.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarketGossip.Shared.IntegrationBus.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMqConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(RabbitMqConfig.Section);

        services.Configure<RabbitMqConfig>(section);

        return services;
    }

    public static IServiceCollection AddRabbitMqClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddRabbitMqConfig(configuration);

        services.AddSingleton<IRabbitMqClient>(serviceProvider => new RabbitMqClient(
            serviceProvider.GetRequiredService<IOptions<RabbitMqConfig>>(),
            serviceProvider.GetRequiredService<ILogger<RabbitMqClient>>()
        ));

        return services;
    }

    public static IServiceCollection AddIntegrationBusConsumer(this IServiceCollection services)
    {
        services.AddSingleton<IIntegrationBusConsumer>(serviceProvider => new IntegrationBusConsumer(
            serviceProvider.GetRequiredService<IRabbitMqClient>(),
            serviceProvider.GetRequiredService<IServiceScopeFactory>(),
            serviceProvider.GetRequiredService<ILogger<IntegrationBusConsumer>>()
        ));

        return services;
    }

    public static IServiceCollection AddIntegrationBusPublisher(this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        switch (lifetime)
        {
            case ServiceLifetime.Scoped:
                services.AddScoped<IIntegrationBusPublisher>(serviceProvider => new IntegrationBusPublisher(
                    serviceProvider.GetRequiredService<IRabbitMqClient>(),
                    serviceProvider.GetRequiredService<ILogger<IntegrationBusPublisher>>()));

                return services;
            case ServiceLifetime.Transient:
                services.AddTransient<IIntegrationBusPublisher>(serviceProvider => new IntegrationBusPublisher(
                    serviceProvider.GetRequiredService<IRabbitMqClient>(),
                    serviceProvider.GetRequiredService<ILogger<IntegrationBusPublisher>>()));

                return services;
            case ServiceLifetime.Singleton:
                services.AddSingleton<IIntegrationBusPublisher>(serviceProvider => new IntegrationBusPublisher(
                    serviceProvider.GetRequiredService<IRabbitMqClient>(),
                    serviceProvider.GetRequiredService<ILogger<IntegrationBusPublisher>>()));

                return services;
            default:
                return services;
        }
    }
}