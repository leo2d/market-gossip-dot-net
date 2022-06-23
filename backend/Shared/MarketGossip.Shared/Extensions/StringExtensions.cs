using System.Text.Json;

namespace MarketGossip.Shared.Extensions;

public static class StringExtensions
{
    public static decimal GetDecimalOrDefault(this string str, decimal defaultValue = 0)
    {
        if (string.IsNullOrWhiteSpace(str)) return defaultValue;

        return decimal.TryParse(str, out var parsedValue) ? parsedValue : defaultValue;
    }

    public static long GetLongOrDefault(this string str, long defaultValue = 0)
    {
        if (string.IsNullOrWhiteSpace(str)) return defaultValue;

        return long.TryParse(str, out var parsedValue) ? parsedValue : defaultValue;
    }

    public static bool TryDeserializeFromJson<T>(this string jsonStr, out T? obj)
    {
        obj = default;

        if (string.IsNullOrWhiteSpace(jsonStr))
            return false;

        try
        {
            obj = JsonSerializer.Deserialize<T>(jsonStr);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}