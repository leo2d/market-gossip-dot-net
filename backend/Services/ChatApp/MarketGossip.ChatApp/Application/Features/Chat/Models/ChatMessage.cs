namespace MarketGossip.ChatApp.Application.Features.Chat.Models;

public record ChatMessage
{
    public string Id { get; init; }
    public string Author { get; init; }
    public string AuthorColor { get; init; } = "E8E9A1";
    public string Text { get; init; }
    public DateTime SentAt { get; set; }
    public string? RoomId { get; set; }
}