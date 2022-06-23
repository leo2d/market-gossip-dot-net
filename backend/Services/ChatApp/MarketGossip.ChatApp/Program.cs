using System.Reflection;
using MarketGossip.ChatApp.Application.Extensions;
using MarketGossip.ChatApp.Application.Features.Chat;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.SetupConfigurations(configuration);
builder.Services.SetupDatabase(configuration);

builder.Services.SetupIdentity(configuration);
builder.Services.SetupJwtAuthentication(configuration);

builder.Services.AddAuthorization();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.SetupIntegrationBus(configuration);
builder.Services.SetupIntegrationEventHandlers();

const string chatPolicy = "ChatPolicy";
builder.Services.SetupCorsPolicy(chatPolicy, configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.UseCors(chatPolicy);

app.StartListeningToIntegrationEvents(configuration);

app.Run();