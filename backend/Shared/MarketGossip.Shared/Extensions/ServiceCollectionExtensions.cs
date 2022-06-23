using MarketGossip.Shared.Options;
using MarketGossip.Shared.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace MarketGossip.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMqConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(RabbitMqConfig.Section);

        services.Configure<RabbitMqConfig>(section);

        return services;
    }  
    
    public static IServiceCollection SetupIntegrationBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRabbitMqConfig(configuration);
        
        services.AddSingleton(serviceProvider => new IntegrationEventHandler(
            serviceProvider.GetRequiredService<IServiceScopeFactory>(),
            serviceProvider.GetRequiredService<IOptions<RabbitMqConfig>>(),
            serviceProvider.GetRequiredService<ILogger<IntegrationEventHandler>>()
        ));

        return services;
    }
}