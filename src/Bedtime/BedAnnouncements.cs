using System;
using System.Collections.Generic;
using System.Text;

namespace Bedtime;

internal sealed class BedAnnouncements
{
    private bool _announcedEveryoneAsleep;

    public string? Update(int inBed, IReadOnlyCollection<string> awakePlayers, bool showAwakePlayers = true)
    {
        int playerCount = inBed + awakePlayers.Count;
        if (playerCount <= 1 || inBed == 0)
        {
            Clear();
            return null;
        }

        if (awakePlayers.Count == 0)
        {
            if (_announcedEveryoneAsleep)
                return null;
            _announcedEveryoneAsleep = true;
            return "Everyone went to sleep. Sweet dreams!";
        }

        _announcedEveryoneAsleep = false;
        var message = new StringBuilder();
        // Vanilla vertically centers notifications. Balance the list with blank lines
        // above the count so it stays below the hotbar as the list grows downward.
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

        // Repeating on vanilla's sleep-update pass refreshes the client's fade.
        return message.ToString();
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
        _announcedEveryoneAsleep = false;
    }
}
