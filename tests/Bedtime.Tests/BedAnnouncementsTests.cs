using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bedtime.Tests;

[TestClass]
public sealed class BedAnnouncementsTests
{
    [TestMethod]
    public void SoloSleepStaysSilentAndJoiningPlayersEnableAnnouncements()
    {
        var announcements = new BedAnnouncements();
        Assert.IsNull(announcements.Update(1, Array.Empty<string>()));
        Assert.IsNull(announcements.Update(1, Array.Empty<string>()));
        Assert.AreEqual("1 of 2 players asleep",
            announcements.Update(1, new[] { "Bob" }, showAwakePlayers: false));
        Assert.IsNull(announcements.Update(1, Array.Empty<string>()));
    }

    [TestMethod]
    public void EveryoneAsleepCanBeAnnouncedAgainAfterPlayersLeaveBedOrSleepEnds()
    {
        var announcements = new BedAnnouncements();
        Assert.AreEqual("Everyone went to sleep. Sweet dreams!",
            announcements.Update(2, Array.Empty<string>()));
        Assert.IsNull(announcements.Update(2, Array.Empty<string>()));
        announcements.Update(1, new[] { "Bob" });
        Assert.AreEqual("Everyone went to sleep. Sweet dreams!",
            announcements.Update(2, Array.Empty<string>()));
        announcements.Clear();
        Assert.AreEqual("Everyone went to sleep. Sweet dreams!",
            announcements.Update(2, Array.Empty<string>()));
    }

    [TestMethod]
    public void CountOnlyOmitsTheList()
    {
        var announcements = new BedAnnouncements();

        Assert.AreEqual("1 of 3 players asleep",
            announcements.Update(1, new[] { "Bob", "Charlie" }, showAwakePlayers: false));
    }

    [TestMethod]
    public void CountOnlySuppressesUnchangedCounts()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, new[] { "Bob" }, showAwakePlayers: false);

        Assert.IsNull(announcements.Update(1, new[] { "Charlie" }, showAwakePlayers: false));
        Assert.AreEqual("1 of 3 players asleep",
            announcements.Update(1, new[] { "Bob", "Charlie" }, showAwakePlayers: false));
        Assert.AreEqual("Everyone went to sleep. Sweet dreams!",
            announcements.Update(3, Array.Empty<string>(), showAwakePlayers: false));
    }

    [TestMethod]
    public void CountOnlyStopsWhenTheLastSleeperLeaves()
    {
        var announcements = new BedAnnouncements();
        Assert.IsNull(announcements.Update(0, new[] { "Bob" }, showAwakePlayers: false));
        announcements.Update(1, new[] { "Alice" }, showAwakePlayers: false);

        Assert.IsNull(announcements.Update(0, new[] { "Bob", "Alice" }, showAwakePlayers: false));
        Assert.IsNull(announcements.Update(0, new[] { "Bob" }, showAwakePlayers: false));
    }

    [TestMethod]
    public void ListsAwakePlayersUnderTheCount()
    {
        var announcements = new BedAnnouncements();

        Assert.AreEqual("\n\n\n1 of 3 players asleep\nNot Sleeping:\n• Bob\n• Charlie",
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
    public void SuppressesIdenticalMessagesRegardlessOfPlayerOrder()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, new[] { "Bob", "Charlie" });

        Assert.IsNull(announcements.Update(1, new[] { "Bob", "Charlie" }));
        Assert.IsNull(announcements.Update(1, new[] { "Charlie", "Bob" }));
    }

    [TestMethod]
    public void ExplicitReplayRepeatsUnchangedStatusWithoutEnablingAutomaticRepeats()
    {
        var announcements = new BedAnnouncements();
        string? initial = announcements.Update(1, new[] { "Bob" });
        Assert.IsNotNull(initial);
        Assert.IsNull(announcements.Update(1, new[] { "Bob" }));
        Assert.AreEqual(initial, announcements.Update(1, new[] { "Bob" }, repeat: true));
        Assert.IsNull(announcements.Update(1, new[] { "Bob" }));
    }

    [TestMethod]
    public void ReplayStillStaysSilentForSoloPlayersAndNobodyInBed()
    {
        var announcements = new BedAnnouncements();
        Assert.IsNull(announcements.Update(1, Array.Empty<string>(), repeat: true));
        Assert.IsNull(announcements.Update(0, new[] { "Bob", "Charlie" }, repeat: true));
    }

    [TestMethod]
    public void CombinesSimultaneousBedEntriesIntoOneAnnouncement()
    {
        var announcements = new BedAnnouncements();
        Assert.AreEqual("\n\n2 of 3 players asleep\nNot Sleeping:\n• Charlie",
            announcements.Update(2, new[] { "Charlie" }));
    }

    [TestMethod]
    public void AnnouncesEveryoneAsleepOnceWithoutAList()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, new[] { "Bob" });

        Assert.AreEqual("Everyone went to sleep. Sweet dreams!",
            announcements.Update(2, Array.Empty<string>()));
        Assert.IsNull(announcements.Update(2, Array.Empty<string>()));
    }

    [TestMethod]
    public void UpdatesTheListWhenSleepersSwapButTheCountIsUnchanged()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, new[] { "Bob" });

        Assert.AreEqual("\n\n1 of 2 players asleep\nNot Sleeping:\n• Alice",
            announcements.Update(1, new[] { "Alice" }));
    }

    [TestMethod]
    public void UpdatesWhenPlayersLeaveBedIncludingTheLastSleeper()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(2, Array.Empty<string>());

        Assert.AreEqual("\n\n1 of 2 players asleep\nNot Sleeping:\n• Bob",
            announcements.Update(1, new[] { "Bob" }));
        Assert.IsNull(announcements.Update(0, new[] { "Alice", "Bob" }));
        Assert.IsNull(announcements.Update(0, new[] { "Alice", "Bob" }));
    }

    [TestMethod]
    public void UpdatesWhenAwakePlayersJoinOrLeave()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, new[] { "Bob" });

        Assert.AreEqual("\n\n\n1 of 3 players asleep\nNot Sleeping:\n• Bob\n• Charlie",
            announcements.Update(1, new[] { "Bob", "Charlie" }));
        Assert.AreEqual("\n\n1 of 2 players asleep\nNot Sleeping:\n• Bob",
            announcements.Update(1, new[] { "Bob" }));
    }

    [TestMethod]
    public void StopsWhenOnlyOnePlayerRemains()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, new[] { "Bob" });

        Assert.IsNull(announcements.Update(0, new[] { "Bob" }));
        Assert.IsNull(announcements.Update(0, Array.Empty<string>()));
        Assert.IsNull(announcements.Update(1, Array.Empty<string>()));
    }

    [TestMethod]
    public void ResetAfterSleepCanAnnounceTheSamePlayersAgain()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, new[] { "Bob" });
        announcements.Clear();

        Assert.AreEqual("\n\n1 of 2 players asleep\nNot Sleeping:\n• Bob",
            announcements.Update(1, new[] { "Bob" }));
    }

    [TestMethod]
    public void PreservesDuplicateAndUnicodeNames()
    {
        var announcements = new BedAnnouncements();

        Assert.AreEqual("\n\n\n\n1 of 4 players asleep\nNot Sleeping:\n• Björn\n• Bob\n• Bob",
            announcements.Update(1, new[] { "Bob", "Björn", "Bob" }));
    }

    [TestMethod]
    public void KeepsNamesOnOneLineWithoutMarkupOrLocalizationTokens()
    {
        var announcements = new BedAnnouncements();

        Assert.AreEqual("\n\n1 of 2 players asleep\nNot Sleeping:\n• ‹b›Bob‹/b› ＄name ＼n",
            announcements.Update(1, new[] { "<b>Bob</b>\n$name\t\\n" }));
    }

    [TestMethod]
    public void LabelsMissingNamesWithoutOmittingPlayers()
    {
        var announcements = new BedAnnouncements();

        Assert.AreEqual("\n\n\n1 of 3 players asleep\nNot Sleeping:\n• Unknown player\n• Unknown player",
            announcements.Update(1, new[] { "", " \r\n" }));
    }
}
