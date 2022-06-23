using MarketGossip.Shared.IntegrationBus.Contracts;
using MarketGossip.Shared.IntegrationBus.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MarketGossip.Shared.IntegrationBus;

public class RabbitMqClient : IRabbitMqClient, IDisposable
{
    private readonly ILogger<RabbitMqClient> _logger;

    private readonly Dictionary<Type, IModel> _channels;
    private IConnection? _connection;

    private static readonly object Padlock = new();

    public RabbitMqClient(
        IOptions<RabbitMqConfig> rabbitMqOptions,
        ILogger<RabbitMqClient> logger)
    {
        var rabbitMqConfig = rabbitMqOptions.Value ?? throw new ArgumentNullException(nameof(rabbitMqOptions));

        _logger = logger;

        var factory = new ConnectionFactory
        {
            Port = rabbitMqConfig.Port,
            HostName = rabbitMqConfig.HostName,
            UserName = rabbitMqConfig.UserName,
            Password = rabbitMqConfig.Password,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channels = new Dictionary<Type, IModel>();
    }

    public IModel GetOrCreateChannelForEvent<TEvent>(string? queue = null)
    {
        var eventType = typeof(TEvent);

        try
        {
            lock (Padlock)
            {
                if (_channels.TryGetValue(eventType, out var channel))
                    return channel;

                if (string.IsNullOrEmpty(queue))
                    throw new ArgumentNullException($"{nameof(queue)} is required to create a new channel");

                var newChannel = _connection!.CreateModel();

                newChannel.QueueDeclare(queue,
                    durable: false,
                    exclusive: false,
                    autoDelete: false);

                _channels.Add(eventType, newChannel);

                return newChannel;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void Dispose()
    {
        try
        {
            _connection?.Close();
            _connection?.Dispose();
            _connection = null;

            foreach (var (_, channel) in _channels)
            {
                channel?.Close();
                channel?.Dispose();
            }

            _channels.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Cannot dispose RabbitMQ channel or connection");
        }
    }
}