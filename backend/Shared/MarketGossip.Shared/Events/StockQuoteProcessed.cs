namespace MarketGossip.Shared.Events;

public record StockQuoteProcessed : IntegrationEvent
{
    public string Symbol { get; init; }
    public decimal QuoteValue { get; init; }
}