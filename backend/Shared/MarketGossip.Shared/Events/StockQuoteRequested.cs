namespace MarketGossip.Shared.Events;

public record StockQuoteRequested : IntegrationEvent
{
    public string Symbol { get; init; }
    public string? RoomId { get; set; }
}