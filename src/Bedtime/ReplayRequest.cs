namespace Bedtime;

internal sealed class ReplayRequest
{
    private double _nextAllowedAt;

    public static bool Matches(string text)
    {
        string command = text.Trim();
        if (command.Length < 3)
            return false;
        foreach (char character in command)
            if (character is not ('z' or 'Z'))
                return false;
        return true;
    }

    public bool TryAccept(double now)
    {
        // Vanilla may send one copy per recipient. A shared cooldown also keeps
        // simultaneous requests from repeatedly interrupting everyone's HUD.
        if (now < _nextAllowedAt)
            return false;
        _nextAllowedAt = now + 5;
        return true;
    }
}
