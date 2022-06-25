using MarketGossip.ChatApp.Application.Features.Chat.Models;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.IntegrationBus.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace MarketGossip.ChatApp.Application.Features.Chat.EventHandling;

public class StockQuoteProcessedEventHandler : IIntegrationEventHandler<StockQuoteProcessed>
{
    private readonly ILogger<StockQuoteProcessedEventHandler> _logger;
    private readonly IHubContext<ChatHub, IChatClient> _hubContext;

    public StockQuoteProcessedEventHandler(
        ILogger<StockQuoteProcessedEventHandler> logger,
        IHubContext<ChatHub, IChatClient> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task Handle(StockQuoteProcessed @event)
    {
        _logger.LogInformation("Stock quote processed {StockCode}", @event.Symbol);

        var message = CreateResponseMessage(@event);

        await _hubContext.Clients.All.ReceiveMessage(message);
    }

    private static BotMessage CreateResponseMessage(StockQuoteProcessed @event)
    {
        if (!@event.Success || @event.StockInfo is null)
        {
            return new BotMessage
            {
                Author = AppConstants.ChatBotName,
                SentAt = DateTime.Now,
                Id = Guid.NewGuid().ToString(),
                Text = $"It was not possible to process the the StockCode {@event.Symbol}",
                Type = BotMessageType.Warning
            };
        }

        return new BotMessage
        {
            Author = AppConstants.ChatBotName,
            SentAt = DateTime.Now,
            Id = Guid.NewGuid().ToString(),
            Text = $"{@event.Symbol} quote is ${@event.StockInfo.GetQuoteValue()} per share",
            Type = BotMessageType.Success
        };
    }
}