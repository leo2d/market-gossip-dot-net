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
        _chatBot = new ChatBot(AppConstants.ChatBotName, new[] { "stock" });
    }

    public async Task SendWelcomeGreetings(ChatMessage message)
    {
        var systemMessage = message with
        {
            Author = AppConstants.SystemSenderName,
            Text = $"{message.Author} joined the room"
        };

        await Clients.All.ReceiveMessage(systemMessage);

        var welcomeMessage = $"Welcome {message.Author}! I'm {_chatBot.Name} and I'm here to help";

        await Clients.Caller.ReceiveMessage(CreateBotMessage(welcomeMessage));
    }

    public async Task SendMessage(ChatMessage message)
    {
        await Clients.All.ReceiveMessage(message);
    }

    public async Task SendCommand(ChatMessage message)
    {
        if (!_chatBot.IsAValidCommand(ChatCommandHelper.GetCommandFromMessage(message.Text)))
        {
            await Clients.Caller.ReceiveMessage(CreateBotMessage("Invalid Command =| ", BotMessageType.Error));
            return;
        }

        var stockCode = ChatCommandHelper.GetCommandArgs(message.Text);

        var result = await _mediator.Send(new BotRequestStockQuote(stockCode, message.RoomId, message.Author));

        if (result.IsFailure)
        {
            await Clients.Caller.ReceiveMessage(CreateBotMessage(result.Error, BotMessageType.Error));
            return;
        }

        //if the command is wrong we only send the message to the user who sent it
        //if it is correct we sent the message to the room
        await Clients.All.ReceiveMessage(
            CreateBotMessage("Command Received, I'll be back soon with the results . . ."));
    }

    private BotMessage CreateBotMessage(string text, string type = BotMessageType.Regular) => new()
    {
        Text = text,
        Author = _chatBot.Name,
        SentAt = DateTime.Now,
        Id = Guid.NewGuid().ToString(),
        Type = type
    };
}