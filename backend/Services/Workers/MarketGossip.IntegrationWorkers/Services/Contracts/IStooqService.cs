using MarketGossip.IntegrationWorkers.Dtos;
using MarketGossip.Shared.Dtos.Result;

namespace MarketGossip.IntegrationWorkers.Services.Contracts;

public interface IStooqService
{
    Task<Result<StockInfo>> GetStockInfo(string stockCode);
}