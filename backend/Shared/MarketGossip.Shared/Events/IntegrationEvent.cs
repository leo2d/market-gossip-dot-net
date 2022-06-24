namespace MarketGossip.Shared.Events;

public abstract record IntegrationEvent()
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; init; } = DateTime.Now;
}