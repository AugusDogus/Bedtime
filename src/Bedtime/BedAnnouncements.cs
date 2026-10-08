using System;
using System.Collections.Generic;
using System.Text;

namespace Bedtime;

internal sealed class BedAnnouncements
{
    private string? _lastMessage;

    public string? Update(int inBed, IReadOnlyCollection<string> awakePlayers, bool showAwakePlayers = true, bool repeat = false)
    {
        int playerCount = inBed + awakePlayers.Count;
        if (playerCount <= 1 || inBed == 0)
        {
            Clear();
            return null;
        }

        if (awakePlayers.Count == 0)
        {
            return ChangedMessage("Everyone went to sleep. Sweet dreams!", repeat);
        }

        var message = new StringBuilder();
        // Balance vanilla's vertically centered notification below the hotbar.
        if (showAwakePlayers)
            message.Append('\n', awakePlayers.Count + 1);
        message.Append($"{inBed} of {playerCount} players asleep");
        if (showAwakePlayers)
        {
            var names = new List<string>(awakePlayers.Count);
            foreach (string name in awakePlayers)
                names.Add(DisplayName(name));
            names.Sort(StringComparer.Ordinal);

            message.Append("\nNot Sleeping:");
            foreach (string name in names)
                message.Append("\n• ").Append(name);
        }

        return ChangedMessage(message.ToString(), repeat);
    }

    private string? ChangedMessage(string message, bool repeat)
    {
        if (!repeat && message == _lastMessage)
            return null;
        _lastMessage = message;
        return message;
    }

    private static string DisplayName(string name)
    {
        // Names pass through localization and rich-text parsing on vanilla clients.
        // Keep each name on one line and prevent it from becoming markup or a token.
        var text = new StringBuilder(name.Length);
        foreach (char character in name)
        {
            text.Append(character switch
            {
                '<' => '‹',
                '>' => '›',
                '$' => '＄',
                '\\' => '＼',
                _ when char.IsControl(character) || char.IsWhiteSpace(character) => ' ',
                _ => character
            });
        }
        string result = text.ToString().Trim();
        return result.Length == 0 ? "Unknown player" : result;
    }

    public void Clear()
    {
        _lastMessage = null;
    }
}
