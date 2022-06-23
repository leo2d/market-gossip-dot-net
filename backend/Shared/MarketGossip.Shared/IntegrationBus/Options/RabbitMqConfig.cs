namespace MarketGossip.Shared.IntegrationBus.Options;

public record RabbitMqConfig
{
    public const string Section = "RabbitMq";

    public int Port { get; init; }
    public string HostName { get; init; }
    public string UserName { get; init; }
    public string Password { get; init; }
}