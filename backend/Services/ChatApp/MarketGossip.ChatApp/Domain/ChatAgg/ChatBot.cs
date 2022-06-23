namespace MarketGossip.ChatApp.Domain.Bot;

public class ChatBot
{
    public string Name { get; }

    private HashSet<string> _validCommands;
    public IReadOnlySet<string> ValidCommands => _validCommands;

    public ChatBot(string name, IEnumerable<string> validCommands)
    {
        Name = name;

        _validCommands = validCommands.ToHashSet();
    }

    public bool IsAValidCommand(string command) => _validCommands.Contains(command);
}