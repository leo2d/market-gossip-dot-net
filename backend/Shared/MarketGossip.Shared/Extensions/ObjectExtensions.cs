using System.Text.Json;

namespace MarketGossip.Shared.Extensions;

public static class ObjectExtensions
{
    public static string ToJson(this object obj)
    {
        var objJson = JsonSerializer.Serialize(obj);

        return objJson;
    }
}