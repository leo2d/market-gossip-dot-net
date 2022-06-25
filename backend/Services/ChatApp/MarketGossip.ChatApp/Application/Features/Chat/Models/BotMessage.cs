namespace MarketGossip.ChatApp.Application.Features.Chat.Models;

public static class BotMessageType
{
    public const string Success = "success";
    public const string Error = "error";
    public const string Warning = "warning";
    public const string Regular = "regular";
}

public record BotMessage : ChatMessage
{
    public string Type { get; init; } = BotMessageType.Regular;
};