namespace MarketGossip.Shared.Events;

public record StockQuoteProcessed : IntegrationEvent
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public string Symbol { get; init; }

    public StockData? StockInfo { get; set; }

    public record StockData
    {
        public string? Symbol { get; init; }
        public string? Date { get; init; }
        public string? Time { get; init; }
        public decimal Open { get; init; }
        public decimal High { get; init; }
        public decimal Low { get; init; }
        public decimal Close { get; init; }
        public long Volume { get; init; }
        
        public decimal GetQuoteValue() => Close;
    }
}