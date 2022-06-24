using MarketGossip.IntegrationWorkers.Dtos;
using MarketGossip.IntegrationWorkers.Services.Contracts;
using MarketGossip.Shared.Dtos.Result;
using MarketGossip.Shared.Extensions;
using MarketGossip.Shared.Helpers;

namespace MarketGossip.IntegrationWorkers.Services;

public class StooqService : IStooqService
{
    private readonly HttpClient _httpClient;

    public StooqService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<StockInfo>> GetStockInfo(string stockCode)
    {
        try
        {
            var url = $"https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv";

            var response = await _httpClient.GetAsync(url);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return Result.Failure<StockInfo>($"Cant reach the Service now. Reason {responseContent}");

            var firstLineValues = CsvHelper.GetFirstLineValuesAsArray(responseContent);

            var notFoundError = $"There's no data for the StockCode {stockCode}";

            if (firstLineValues is null)
                return Result.Failure<StockInfo>(notFoundError);

            //Even if we pass a code that does not exists, the api will return a csv
            //with all the headers and the first column(Symbol) filled, and "N/D" for the other columns
            //So we need to check if all the other columns are valid
            var invalidValuesOnly = firstLineValues
                .Skip(1)
                .All(v => string.IsNullOrWhiteSpace(v.Replace("N/D", string.Empty).Trim()));

            if (invalidValuesOnly)
                return Result.Failure<StockInfo>(notFoundError);

            var parseResult = MountStockResponse(firstLineValues);

            return parseResult;
        }
        catch (Exception e)
        {
            return Result.Failure<StockInfo>(e.Message);
        }
    }

    private static Result<StockInfo> MountStockResponse(IReadOnlyList<string> values)
    {
        if (values?.Count < 8) return Result.Failure<StockInfo>("Invalid data");

        var response = new StockInfo
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