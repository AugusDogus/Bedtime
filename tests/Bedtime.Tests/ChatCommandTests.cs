using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splatform;
using UnityEngine;

namespace Bedtime.Tests;

[TestClass]
public sealed class ChatCommandTests
{
    [DataTestMethod]
    [DataRow(false, 0)]
    [DataRow(false, 1)]
    [DataRow(false, 2)]
    [DataRow(true, 2)]
    public void RecognizesVanillaChatRoutesWithoutChangingThePacket(bool global, int type)
    {
        ZPackage packet = Packet(global, "zzz", type);
        byte[] original = packet.GetArray();
        Assert.IsTrue(ChatCommand.IsReplay(packet, 123));
        Assert.AreEqual(0, packet.GetPos());
        CollectionAssert.AreEqual(original, packet.GetArray());
    }

    [TestMethod]
    public void DoesNotTreatOtherMessagesPingsOrForgedSendersAsCommands()
    {
        Assert.IsFalse(ChatCommand.IsReplay(Packet(false, "hello"), 123));
        Assert.IsFalse(ChatCommand.IsReplay(Packet(false, "zzz please"), 123));
        Assert.IsFalse(ChatCommand.IsReplay(Packet(true, "zzz", 3), 123));
        Assert.IsFalse(ChatCommand.IsReplay(Packet(false, "zzz"), 456));
    }

    [TestMethod]
    public void TruncatedPacketsAreNotConsumedOrTreatedAsCommands()
    {
        byte[] bytes = Packet(true, "zzz").GetArray();
        for (int length = 0; length < bytes.Length; length++)
        {
            var truncated = new byte[length];
            Array.Copy(bytes, truncated, length);
            var packet = new ZPackage(truncated);
            Assert.IsFalse(ChatCommand.IsReplay(packet, 123), $"Accepted a packet truncated at {length} bytes.");
            Assert.AreEqual(0, packet.GetPos());
        }
    }

    private static ZPackage Packet(bool global, string text, int type = 1)
    {
        var parameters = new ZPackage();
        if (global)
            parameters.Write(Vector3.zero);
        parameters.Write(type);
        new UserInfo { Name = "Bob", UserId = new PlatformUserID("Steam", "123") }.Serialize(ref parameters);
        parameters.Write(text);
        var data = new ZRoutedRpc.RoutedRPCData
        {
            m_msgID = 1,
            m_senderPeerID = 123,
            m_targetPeerID = 456,
            m_targetZDO = global ? ZDOID.None : new ZDOID(123, 1),
            m_methodHash = (global ? "ChatMessage" : "Say").GetStableHashCode(),
            m_parameters = parameters,
        };
        var packet = new ZPackage();
        data.Serialize(packet);
        packet.SetPos(0);
        return packet;
    }
}
