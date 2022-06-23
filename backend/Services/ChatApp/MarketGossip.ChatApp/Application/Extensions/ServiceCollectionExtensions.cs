using System.Text;
using MarketGossip.ChatApp.Application.Features.Chat.EventHandling;
using MarketGossip.ChatApp.Application.Options;
using MarketGossip.ChatApp.Infrastructure.Data;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.IntegrationBus.Contracts;
using MarketGossip.Shared.IntegrationBus.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MarketGossip.ChatApp.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("MarketGossipAppDb")));

        return services;
    }

    public static IServiceCollection SetupIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection SetupJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                };
            });

        return services;
    }

    public static IServiceCollection SetupIntegrationBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRabbitMqClient(configuration)
            .AddIntegrationBusConsumer()
            .AddIntegrationBusPublisher();

        return services;
    }

    public static IServiceCollection SetupIntegrationEventHandlers(this IServiceCollection services)
    {
        services.AddTransient<IIntegrationEventHandler<StockQuoteProcessed>, StockQuoteProcessedEventHandler>();

        return services;
    }

    public static IServiceCollection SetupCorsPolicy(this IServiceCollection services,
        string policyName, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(policyName, policy =>
            {
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins(configuration["Cors:ChatOrigin"])
                    .AllowCredentials();
            });
        });
        return services;
    }

    public static IServiceCollection SetupConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        var eventQueuesSection = configuration.GetSection(EventQueuesConfig.Section);

        services.Configure<EventQueuesConfig>(eventQueuesSection);

        return services;
    }
}