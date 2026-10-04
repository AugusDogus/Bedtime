using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bedtime.Tests;

[TestClass]
public sealed class BedAnnouncementsTests
{
    [TestMethod]
    public void AnnouncesHowManyPlayersAreInBed()
    {
        var announcements = new BedAnnouncements();

        string? message = announcements.Update(1, 3);

        Assert.AreEqual("1 out of 3 players are in bed.", message);
    }

    [TestMethod]
    public void SaysNothingWhenNobodyIsInBed()
    {
        var announcements = new BedAnnouncements();
        Assert.IsNull(announcements.Update(0, 0));
        Assert.IsNull(announcements.Update(0, 1));
        Assert.IsNull(announcements.Update(0, 2));
    }

    [TestMethod]
    public void DoesNotRepeatUnchangedCounts()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, 2);

        Assert.IsNull(announcements.Update(1, 2));
        Assert.IsNull(announcements.Update(1, 2));
    }

    [TestMethod]
    public void CombinesSimultaneousBedEntriesIntoOneAnnouncement()
    {
        var announcements = new BedAnnouncements();
        Assert.AreEqual("2 out of 3 players are in bed.",
            announcements.Update(2, 3));
    }

    [TestMethod]
    public void AnnouncesTheLastPlayerEnteringBed()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, 2);

        Assert.AreEqual("2 out of 2 players are in bed.",
            announcements.Update(2, 2));
    }

    [TestMethod]
    public void StaysQuietWhenSleepersSwapButTheCountIsUnchanged()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, 2);

        Assert.IsNull(announcements.Update(1, 2));
    }

    [TestMethod]
    public void UpdatesCountsWhenPlayersLeaveBedIncludingTheLastSleeper()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(2, 2);

        Assert.AreEqual("1 out of 2 players are in bed.",
            announcements.Update(1, 2));
        Assert.AreEqual("0 out of 2 players are in bed.",
            announcements.Update(0, 2));
        Assert.IsNull(announcements.Update(0, 2));
    }

    [TestMethod]
    public void RecalculatesWhenAwakePlayersJoinOrLeave()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, 2);

        Assert.AreEqual("1 out of 3 players are in bed.",
            announcements.Update(1, 3));
        Assert.AreEqual("1 out of 2 players are in bed.",
            announcements.Update(1, 2));
    }

    [TestMethod]
    public void DisconnectedSleepersAreRemovedFromTheCount()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, 2);

        Assert.AreEqual("0 out of 1 player is in bed.", announcements.Update(0, 1));
        Assert.IsNull(announcements.Update(0, 0));
        Assert.AreEqual("1 out of 1 player is in bed.", announcements.Update(1, 1));
    }

    [TestMethod]
    public void ResetAfterSleepCanAnnounceTheSamePlayersAgain()
    {
        var announcements = new BedAnnouncements();
        announcements.Update(1, 1);
        announcements.Clear();

        Assert.AreEqual("1 out of 1 player is in bed.", announcements.Update(1, 1));
    }

}
