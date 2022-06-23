using System.Reflection;
using MarketGossip.ChatApp;
using MarketGossip.ChatApp.Application.Extensions;
using MarketGossip.ChatApp.Application.Features.Chat;
using MarketGossip.ChatApp.Application.Features.Chat.EventHandling;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.ServiceBus;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.SetupDatabase(configuration);

builder.Services.SetupIdentity(configuration);

builder.Services.SetupJwtAuthentication(configuration);
builder.Services.AddAuthorization();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddSingleton(serviceProvider => new IntegrationEventHandler(
    serviceProvider.GetRequiredService<IServiceScopeFactory>(),
    configuration,
    serviceProvider.GetRequiredService<ILogger<IntegrationEventHandler>>()
));

builder.Services.AddScoped<IIntegrationBus, IntegrationBus>();
builder.Services.AddTransient<StockQuoteProcessedEventHandler>();


const string corsPolicy = "ChatPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:3000")
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.UseCors(corsPolicy);

var eventHandler = app.Services.GetRequiredService<IntegrationEventHandler>();
eventHandler.StartListening<StockQuoteProcessed>(configuration["EventQueues:StockQuoteProcessedQueue"]);
eventHandler.StartListening<StockQuoteRequested>(configuration["EventQueues:StockQuoteRequestedQueue"]);

app.Run();