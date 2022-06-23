using System.Text.RegularExpressions;

namespace MarketGossip.ChatApp.Helpers;

public class ChatCommandHelper
{
    public static string GetCommandFromMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return string.Empty;

        const string pattern = @"(?<=\/).*.(?=\=)";
        const short expectedMatches = 1;

        var matchResult = Regex.Matches(message, pattern, RegexOptions.IgnoreCase);

        return matchResult?.Count is not expectedMatches ? string.Empty : matchResult.First().Value;
    }

    public static string? GetCommandArgs(string message)
    {
        const string pattern = @"(?<=\=).*.";
        const short expectedMatches = 1;

        var matchResult = Regex.Matches(message, pattern, RegexOptions.IgnoreCase);

        if (matchResult?.Count is not expectedMatches) return null;

        var value = matchResult.First().Value.Trim();

        const string specialCharsPattern = "[^a-zA-Z0-9]+$";

        var onlySpecialCharacters = Regex.Matches(value, specialCharsPattern, RegexOptions.IgnoreCase);

        if (onlySpecialCharacters?.Count > 0 || value.Contains(' ') || value.Contains('/'))
            return null;

        return value;
    }
}