using MarketGossip.ChatApp.Application.Options;
using MarketGossip.Shared.Dtos.Result;
using MarketGossip.Shared.Events;
using MarketGossip.Shared.IntegrationBus.Contracts;
using MediatR;
using Microsoft.Extensions.Options;

namespace MarketGossip.ChatApp.Application.Features.Chat.Commands;

public record BotRequestStockQuote(string? StockCode, string? RoomId, string? User)
    : IRequest<Result>;

public class BotRequestStockQuoteHandler : IRequestHandler<BotRequestStockQuote, Result>
{
    private readonly IIntegrationBusPublisher _integrationBusPublisher;
    private readonly ILogger<BotRequestStockQuoteHandler> _logger;
    private readonly EventQueuesConfig _queuesConfig;

    public BotRequestStockQuoteHandler(
        IIntegrationBusPublisher integrationBusPublisher,
        ILogger<BotRequestStockQuoteHandler> logger,
        IOptions<EventQueuesConfig> queuesConfigOptions)
    {
        _queuesConfig = queuesConfigOptions.Value ?? throw new ArgumentNullException(nameof(queuesConfigOptions));

        _integrationBusPublisher = integrationBusPublisher;
        _logger = logger;
    }

    public async Task<Result> Handle(BotRequestStockQuote request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.StockCode))
        {
            _logger.LogWarning("Invalid Symbol/StockCode {StockCode}", request.StockCode);
            return Result.Failure("Invalid Stock Code =| ");
        }

        var requestedEvent = new StockQuoteRequested
        {
            Symbol = request.StockCode,
            RoomId = request.RoomId
        };

        await _integrationBusPublisher.PublishAsync(_queuesConfig.StockQuoteRequested, requestedEvent);

        return Result.Success();
    }
}