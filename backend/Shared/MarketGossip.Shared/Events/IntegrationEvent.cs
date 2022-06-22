namespace MarketGossip.Shared.Events;

public abstract record IntegrationEvent()
{
    public DateTime CreatedAt { get; } = DateTime.Now;
}