namespace MarketGossip.ChatApp.Application.Options;

public record EventQueuesConfig
{
    public const string Section = "EventQueues";
    
    public string StockQuoteRequested { get; init; }
    public string StockQuoteProcessed { get; init; }
}