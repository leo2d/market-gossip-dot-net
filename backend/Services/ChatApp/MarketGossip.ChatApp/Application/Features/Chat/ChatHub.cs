using MarketGossip.ChatApp.Application.Features.Chat.Models;
using MarketGossip.ChatApp.Helpers;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.Extensions;
using MarketGossip.Shared.IntegrationBus.Contracts;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace MarketGossip.ChatApp.Application.Features.Chat;

public interface IChatClient
{
    Task ReceiveMessage(ChatMessage message);
}

public class ChatHub : Hub<IChatClient>
{
    private const string BotName = "Mkt Gossip BOT";
    private readonly IMediator _mediator;
    private readonly IIntegrationBusPublisher _integrationBusPublisher;

    public ChatHub(IMediator mediator, IIntegrationBusPublisher integrationBusPublisher)
    {
        _mediator = mediator;
        _integrationBusPublisher = integrationBusPublisher;
    }

    public async Task SendMessage(ChatMessage message)
    {
        await Clients.All.ReceiveMessage(message);
    }

    public async Task SendCommand(ChatMessage message)
    {
        HashSet<string> validCommands = new() {"stock"};

        if (!validCommands.Contains(ChatBotCommandHelper.GetCommandFromMessage(message.Text)))
        {
            await Clients.All.ReceiveMessage(CreateBotMessage("Invalid Command =| "));
            return;
        }

        var stockCode = ChatBotCommandHelper.GetCommandArgs(message.Text);

        if (stockCode is null)
        {
            await Clients.All.ReceiveMessage(CreateBotMessage("Invalid Stock Code =| "));
            return;
        }

        var requestedEvent = new StockQuoteRequested {Symbol = stockCode!, RoomId = message.RoomId};

        await _integrationBusPublisher.PublishAsync("stock-quote-requested", requestedEvent);

        await Clients.All.ReceiveMessage(
            CreateBotMessage("Command Received, I'll be back soon with the results . . ."));
    }

    private static ChatMessage CreateBotMessage(string text) => new()
    {
        Text = text,
        Author = BotName,
        SentAt = DateTime.Now,
        Id = Guid.NewGuid().ToString()
    };

    private StockInfo? MountStockInfo(IReadOnlyList<string> values)
    {
        if (values.Count < 8) return null;

        return new StockInfo
        {
            Symbol = values[0],
            Date = values[1],
            Time = values[2],
            Open = values[3].GetDecimalOrDefault(),
            High = values[4].GetDecimalOrDefault(),
            Low = values[5].GetDecimalOrDefault(),
            Close = values[6].GetDecimalOrDefault(),
            Volume = values[7].GetLongOrDefault(),
        };
    }
}

public class StockInfo
{
    public string? Symbol { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public long Volume { get; set; }
}