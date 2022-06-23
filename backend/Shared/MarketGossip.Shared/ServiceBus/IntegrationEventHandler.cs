using System.Text;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MarketGossip.Shared.ServiceBus;

public class IntegrationEventHandler : IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<IntegrationEventHandler> _logger;
    private readonly IConfiguration _configuration;

    private Dictionary<Type, IModel> _channels;

    private IConnection _connection;

    public IntegrationEventHandler(IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<IntegrationEventHandler> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:HostName"],
            Port = int.Parse(_configuration["RabbitMq:Port"]),
            UserName = _configuration["RabbitMq:UserName"],
            Password = _configuration["RabbitMq:Password"],
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channels = new Dictionary<Type, IModel>();
    }

    public void StartListening<TEvent>(string queue) where TEvent : IntegrationEvent
    {
        var channel = _connection.CreateModel();

        _channels.Add(typeof(TEvent), channel);

        channel.QueueDeclare(queue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);

        try
        {
            consumer.Received += OnEventReceived<TEvent>;

            channel.BasicConsume(queue, autoAck: false, consumer: consumer);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task OnEventReceived<TEvent>(object model, BasicDeliverEventArgs ea) where TEvent : IntegrationEvent
    {
        var body = ea.Body.ToArray();
        var messageJson = Encoding.UTF8.GetString(body);

        if (messageJson.TryDeserializeFromJson<TEvent>(out var message))
        {
            var scope = _scopeFactory.CreateScope();
            var handler =
                scope.ServiceProvider.GetService<IIntegrationEventHandler<TEvent>>();

            if (handler is null)
                throw new Exception($"There's no configured handler for this event {typeof(TEvent).Name}");

            try
            {
                await handler.Handle(message!);

                var channel = _channels[typeof(TEvent)];
                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
            _channels = null;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Cannot dispose RabbitMQ channel or connection");
        }
    }
}