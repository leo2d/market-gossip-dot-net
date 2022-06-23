using MarketGossip.ChatApp.Application.Features.Chat.Commands;
using MarketGossip.ChatApp.Application.Features.Chat.Models;
using MarketGossip.ChatApp.Domain.Bot;
using MarketGossip.ChatApp.Helpers;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace MarketGossip.ChatApp.Application.Features.Chat;

public interface IChatClient
{
    Task ReceiveMessage(ChatMessage message);
}

public class ChatHub : Hub<IChatClient>
{
    private readonly IMediator _mediator;
    private ChatBot _chatBot;

    public ChatHub(IMediator mediator)
    {
        _mediator = mediator;
        _chatBot = new ChatBot(AppConstants.ChatBotName, new[] {"stock"});
    }

    public async Task SendMessage(ChatMessage message)
    {
        await Clients.All.ReceiveMessage(message);
    }

    public async Task SendCommand(ChatMessage message)
    {
        if (!_chatBot.IsAValidCommand(ChatCommandHelper.GetCommandFromMessage(message.Text)))
        {
            await Clients.All.ReceiveMessage(CreateBotMessage("Invalid Command =| "));
            return;
        }

        var stockCode = ChatCommandHelper.GetCommandArgs(message.Text);

        var result = await _mediator.Send(new BotRequestStockQuote(stockCode, message.RoomId, message.Author));

        if (result.IsFailure)
        {
            await Clients.All.ReceiveMessage(CreateBotMessage(result.Error));
            return;
        }

        await Clients.All.ReceiveMessage(
            CreateBotMessage("Command Received, I'll be back soon with the results . . ."));
    }

    private ChatMessage CreateBotMessage(string text) => new()
    {
        Text = text,
        Author = _chatBot.Name,
        SentAt = DateTime.Now,
        Id = Guid.NewGuid().ToString()
    };
}