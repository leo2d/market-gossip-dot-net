using MarketGossip.Shared.Dtos.Result;
using MarketGossip.Shared.Extensions;
using MarketGossip.Shared.Helpers;

namespace MarketGossip.IntegrationWorkers;

public interface IStooqService
{
    Task<string> GetStockInfoAsCsv(string stockCode);
}

public record StooqResponse()
{
    public string? Symbol { get; init; }
    public string? Date { get; init; }
    public string? Time { get; init; }
    public decimal Open { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal Close { get; init; }
    public long Volume { get; init; }
}

public class StooqService : IStooqService
{
    public async Task<string> GetStockInfoAsCsv(string stockCode)
    {
        return await Task.FromResult(string.Empty);
        var url = $"https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv";

        using var client = new HttpClient();

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return string.Empty;
        }

        var responseContent = await response.Content.ReadAsStringAsync();

        var firstLineValues = CsvHelper.GetFirstLineValuesAsArray(responseContent);

        var parseResult = MountStockResponse(firstLineValues);

        if (parseResult.IsFailure)
        {
            return string.Empty;
        }

        return "";
    }

    private static Result<StooqResponse> MountStockResponse(IReadOnlyList<string> values)
    {
        if (values?.Count < 8) return Result.Failure<StooqResponse>("Invalid data");

        var response = new StooqResponse
        {
            Symbol = values[0],
            Date = values[1],
            Time = values[2],
            Open = values[3].GetDecimalOrDefault(),
            High = values[4].GetDecimalOrDefault(),
            Low = values[5].GetDecimalOrDefault(),
            Close = values[6].GetDecimalOrDefault(),
            Volume = values[7].GetLongOrDefault(),
        };

        return Result.Success(response);
    }
}