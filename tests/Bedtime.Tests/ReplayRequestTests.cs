using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bedtime.Tests;

[TestClass]
public sealed class ReplayRequestTests
{
    [DataTestMethod]
    [DataRow("zzz", true)]
    [DataRow("  ZzZ  ", true)]
    [DataRow("zzzz", true)]
    [DataRow("ZZzzZZzzZZ", true)]
    [DataRow("z", false)]
    [DataRow("zz", false)]
    [DataRow("zzz!", false)]
    [DataRow("zz zz", false)]
    [DataRow("/zzz", false)]
    [DataRow("zzz please", false)]
    [DataRow("time for zzz", false)]
    [DataRow("", false)]
    public void RecognizesOnlyThreeOrMoreZs(string text, bool expected)
    {
        Assert.AreEqual(expected, ReplayRequest.Matches(text));
    }

    [TestMethod]
    public void RecipientCopiesAndSimultaneousRequestsShareACooldown()
    {
        var request = new ReplayRequest();
        Assert.IsTrue(request.TryAccept(100));
        Assert.IsFalse(request.TryAccept(100));
        Assert.IsFalse(request.TryAccept(102));
        Assert.IsFalse(request.TryAccept(104.99));
        Assert.IsTrue(request.TryAccept(105));
        Assert.IsFalse(request.TryAccept(105));
    }
}
