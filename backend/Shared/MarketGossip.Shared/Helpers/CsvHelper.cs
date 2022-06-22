namespace MarketGossip.Shared.Helpers;

public static class CsvHelper
{
    public static string[]? GetFirstLineValuesAsArray(string csv, bool skipHeader = true, char separator = ',')
    {
        var lines = csv.Split(new[] {'\n'},
            StringSplitOptions.RemoveEmptyEntries).Skip(skipHeader ? 1 : 0);

        var firstLine = lines.FirstOrDefault();

        var values = firstLine?.Split(separator);

        return values;
    }
}