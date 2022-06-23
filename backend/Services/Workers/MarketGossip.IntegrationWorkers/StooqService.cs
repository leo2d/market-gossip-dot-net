namespace MarketGossip.IntegrationWorkers;

public interface IStooqService
{
    Task<string> GetStockInfoAsCsv(string stockCode);
}

public class StooqService : IStooqService
{
    public async Task<string> GetStockInfoAsCsv(string stockCode)
    {
        return await Task.FromResult(string.Empty);
        // var url = $"https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv";
        //
        // using var client = new HttpClient();
        //
        // var response = await client.GetStringAsync(url);
        //
        // return response;

        // var info = CsvUtils.ParseFromString(response);
        //
        // if (info is not null)
        // {
        //     var msg = botMessage with {Text = $"{info.Symbol} quote is ${info.Close} per share"};
        //
        //     await Clients.All.ReceiveMessage(msg);
        // }
    }
}