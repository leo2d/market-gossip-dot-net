namespace MarketGossip.IntegrationWorkers.Dtos;

public class StockInfo
{
    public string? Symbol { get; init; }
    public string? Date { get; init; }
    public string? Time { get; init; }
    public decimal Open { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal Close { get; init; }
    public long Volume { get; init; }
}