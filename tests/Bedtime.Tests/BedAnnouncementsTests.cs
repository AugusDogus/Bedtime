using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bedtime.Tests;

[TestClass]
public sealed class BedAnnouncementsTests
{
    [TestMethod]
    public void ListsAwakePlayersUnderTheCount()
    {
        var announcements = new BedAnnouncements();

        Assert.AreEqual("1 out of 3 players are in bed.\nNot Sleeping:\n• Bob\n• Charlie",
            announcements.Update(1, new[] { "Charlie", "Bob" }));
    }

    [TestMethod]
    public void SaysNothingWhenNobodyIsInBed()
    {
        var announcements = new BedAnnouncements();
        Assert.IsNull(announcements.Update(0, Array.Empty<string>()));
        Assert.IsNull(announcements.Update(0, new[] { "Bob" }));
        Assert.IsNull(announcements.Update(0, new[] { "Bob", "Charlie" }));
    }

    [TestMethod]
    public void DoesNotRepeatUnchangedPlayersOrChangesToTheirOrder()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, new[] { "Bob", "Charlie" });

        Assert.IsNull(announcements.Update(1, new[] { "Bob", "Charlie" }));
        Assert.IsNull(announcements.Update(1, new[] { "Charlie", "Bob" }));
    }

    [TestMethod]
    public void CombinesSimultaneousBedEntriesIntoOneAnnouncement()
    {
        var announcements = new BedAnnouncements();
        Assert.AreEqual("2 out of 3 players are in bed.\nNot Sleeping:\n• Charlie",
            announcements.Update(2, new[] { "Charlie" }));
    }

    [TestMethod]
    public void OmitsTheListWhenEveryoneIsInBed()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, new[] { "Bob" });

        Assert.AreEqual("2 out of 2 players are in bed.",
            announcements.Update(2, Array.Empty<string>()));
    }

    [TestMethod]
    public void UpdatesTheListWhenSleepersSwapButTheCountIsUnchanged()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, new[] { "Bob" });

        Assert.AreEqual("1 out of 2 players are in bed.\nNot Sleeping:\n• Alice",
            announcements.Update(1, new[] { "Alice" }));
    }

    [TestMethod]
    public void UpdatesWhenPlayersLeaveBedIncludingTheLastSleeper()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(2, Array.Empty<string>());

        Assert.AreEqual("1 out of 2 players are in bed.\nNot Sleeping:\n• Bob",
            announcements.Update(1, new[] { "Bob" }));
        Assert.AreEqual("0 out of 2 players are in bed.\nNot Sleeping:\n• Alice\n• Bob",
            announcements.Update(0, new[] { "Alice", "Bob" }));
        Assert.IsNull(announcements.Update(0, new[] { "Alice", "Bob" }));
    }

    [TestMethod]
    public void UpdatesWhenAwakePlayersJoinOrLeave()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, new[] { "Bob" });

        Assert.AreEqual("1 out of 3 players are in bed.\nNot Sleeping:\n• Bob\n• Charlie",
            announcements.Update(1, new[] { "Bob", "Charlie" }));
        Assert.AreEqual("1 out of 2 players are in bed.\nNot Sleeping:\n• Bob",
            announcements.Update(1, new[] { "Bob" }));
    }

    [TestMethod]
    public void DisconnectedSleepersAreRemovedFromTheCount()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, new[] { "Bob" });

        Assert.AreEqual("0 out of 1 player is in bed.\nNot Sleeping:\n• Bob",
            announcements.Update(0, new[] { "Bob" }));
        Assert.IsNull(announcements.Update(0, Array.Empty<string>()));
        Assert.AreEqual("1 out of 1 player is in bed.",
            announcements.Update(1, Array.Empty<string>()));
    }

    [TestMethod]
    public void ResetAfterSleepCanAnnounceTheSamePlayersAgain()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, new[] { "Bob" });
        announcements.Clear();

        Assert.AreEqual("1 out of 2 players are in bed.\nNot Sleeping:\n• Bob",
            announcements.Update(1, new[] { "Bob" }));
    }

    [TestMethod]
    public void PreservesDuplicateAndUnicodeNames()
    {
        var announcements = new BedAnnouncements();

        Assert.AreEqual("1 out of 4 players are in bed.\nNot Sleeping:\n• Björn\n• Bob\n• Bob",
            announcements.Update(1, new[] { "Bob", "Björn", "Bob" }));
    }

    [TestMethod]
    public void KeepsNamesOnOneLineWithoutMarkupOrLocalizationTokens()
    {
        var announcements = new BedAnnouncements();

        Assert.AreEqual("1 out of 2 players are in bed.\nNot Sleeping:\n• ‹b›Bob‹/b› ＄name ＼n",
            announcements.Update(1, new[] { "<b>Bob</b>\n$name\t\\n" }));
    }

    [TestMethod]
    public void LabelsMissingNamesWithoutOmittingPlayers()
    {
        var announcements = new BedAnnouncements();

        Assert.AreEqual("1 out of 3 players are in bed.\nNot Sleeping:\n• Unknown player\n• Unknown player",
            announcements.Update(1, new[] { "", " \r\n" }));
    }
}
