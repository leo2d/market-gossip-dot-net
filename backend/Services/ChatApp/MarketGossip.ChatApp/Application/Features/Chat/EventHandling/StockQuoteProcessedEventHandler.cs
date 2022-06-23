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

        var message = new ChatMessage
        {
            Author = AppConstants.ChatBotName,
            SentAt = DateTime.Now,
            Id = Guid.NewGuid().ToString(),
            Text = $"{@event.Symbol} quote is ${@event.QuoteValue} per share"
        };

        await _hubContext.Clients.All.ReceiveMessage(message);
    }
}