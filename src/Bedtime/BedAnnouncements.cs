using System;
using System.Collections.Generic;
using System.Text;

namespace Bedtime;

internal sealed class BedAnnouncements
{
    private int _inBed;
    private string? _lastMessage;

    public string? Update(int inBed, IReadOnlyCollection<string> awakePlayers)
    {
        bool hadSleepers = _inBed > 0;
        _inBed = inBed;

        int playerCount = inBed + awakePlayers.Count;
        if (playerCount == 0 || (!hadSleepers && inBed == 0))
        {
            _lastMessage = null;
            return null;
        }

        string players = playerCount == 1 ? "player is" : "players are";
        var message = new StringBuilder($"{inBed} out of {playerCount} {players} in bed.");
        if (awakePlayers.Count > 0)
        {
            var names = new List<string>(awakePlayers.Count);
            foreach (string name in awakePlayers)
                names.Add(DisplayName(name));
            names.Sort(StringComparer.Ordinal);

            message.Append("\nNot Sleeping:");
            foreach (string name in names)
                message.Append("\n• ").Append(name);
        }

        string text = message.ToString();
        if (text == _lastMessage)
            return null;

        _lastMessage = text;
        return text;
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
        _inBed = 0;
        _lastMessage = null;
    }
}
