using System.Text;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.Extensions;
using MarketGossip.Shared.IntegrationBus.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MarketGossip.Shared.IntegrationBus;

public class IntegrationBusConsumer : IIntegrationBusConsumer
{
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<IntegrationBusConsumer> _logger;

    private readonly HashSet<string> _listeningQueues;
    private static readonly object Padlock = new();

    public IntegrationBusConsumer(IRabbitMqClient rabbitMqClient,
        IServiceScopeFactory scopeFactory,
        ILogger<IntegrationBusConsumer> logger)
    {
        _rabbitMqClient = rabbitMqClient;
        _scopeFactory = scopeFactory;
        _logger = logger;

        _listeningQueues = new HashSet<string>();
    }

    public void StartListening<TEvent>(string queue) where TEvent : IntegrationEvent
    {
        try
        {
            if (_listeningQueues.Contains(queue)) return;

            var channel = _rabbitMqClient.GetOrCreateChannelForEvent<TEvent>(queue);

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

                var channel = _rabbitMqClient.GetOrCreateChannelForEvent<TEvent>();
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
}