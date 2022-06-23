using System.Text;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.Extensions;
using MarketGossip.Shared.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MarketGossip.Shared.ServiceBus;

public class IntegrationEventHandler : IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<IntegrationEventHandler> _logger;

    private readonly HashSet<string> _listeningQueues;
    private readonly Dictionary<Type, IModel> _channels;
    private IConnection? _connection;

    private static readonly object Padlock = new();

    public IntegrationEventHandler(IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqConfig> rabbitMqOptions,
        ILogger<IntegrationEventHandler> logger)
    {
        var rabbitMqConfig = rabbitMqOptions.Value ?? throw new ArgumentNullException(nameof(rabbitMqOptions));

        _scopeFactory = scopeFactory;
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
        _listeningQueues = new HashSet<string>();
    }

    public Task PublishAsync<TEvent>(string queue, TEvent @event) where TEvent : IntegrationEvent
    {
        var channel = GetOrCreateChannelForEvent<TEvent>(queue);

        var message = @event.ToJson();

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "",
            routingKey: queue,
            basicProperties: null,
            body);

        return Task.CompletedTask;
    }

    public void StartListening<TEvent>(string queue) where TEvent : IntegrationEvent
    {
        // var channel = _connection.CreateModel();
        //
        // _channels.Add(typeof(TEvent), channel);

        try
        {
            if (_listeningQueues.Contains(queue)) return;

            var channel = GetOrCreateChannelForEvent<TEvent>(queue);

            lock (Padlock)
            {
                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.Received += OnEventReceived<TEvent>;

                channel.BasicConsume(queue, autoAck: false, consumer);

                _listeningQueues.Add(queue);
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Error on start listening to queue {Queue}", queue);
            throw;
        }
    }

    private IModel GetOrCreateChannelForEvent<TEvent>(string? queue = null)
    {
        var eventType = typeof(TEvent);

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

    private async Task OnEventReceived<TEvent>(object model, BasicDeliverEventArgs ea) where TEvent : IntegrationEvent
    {
        var eventType = typeof(TEvent).Name;

        var body = ea.Body.ToArray();
        var messageJson = Encoding.UTF8.GetString(body);

        if (messageJson.TryDeserializeFromJson<TEvent>(out var message))
        {
            try
            {
                var scope = _scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetService<IIntegrationEventHandler<TEvent>>();

                if (handler is null)
                    throw new Exception($"There's no configured handler for this event {eventType}");

                await handler.Handle(message!);

                var channel = GetOrCreateChannelForEvent<TEvent>();
                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error on Handling event {Event}. Message {Message}",
                    eventType, messageJson);
                throw;
            }
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
            _listeningQueues.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Cannot dispose RabbitMQ channel or connection");
        }
    }
}