namespace Bedtime;

internal sealed class BedAnnouncements
{
    private int _inBed;
    private int _playerCount;

    public string? Update(int inBed, int playerCount)
    {
        bool changed = _inBed != inBed || _playerCount != playerCount;
        bool hadSleepers = _inBed > 0;
        _inBed = inBed;
        _playerCount = playerCount;

        if (!changed || playerCount == 0 || (!hadSleepers && inBed == 0))
            return null;

        string players = playerCount == 1 ? "player is" : "players are";
        return $"{inBed} out of {playerCount} {players} in bed.";
    }

    public void Clear()
    {
        _inBed = 0;
        _playerCount = 0;
    }
}
