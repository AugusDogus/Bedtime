using System;
using System.IO;

namespace Bedtime;

internal static class ChatCommand
{
    private static readonly int SayHash = "Say".GetStableHashCode();
    private static readonly int ChatHash = "ChatMessage".GetStableHashCode();

    public static bool IsReplay(ZPackage packet, long peerId)
    {
        int position = packet.GetPos();
        try
        {
            packet.ReadLong(); // Message ID.
            if (packet.ReadLong() != peerId)
                return false;
            packet.ReadLong(); // Recipient, which may be an individual client.
            ZDOID target = packet.ReadZDOID();
            int method = packet.ReadInt();
            bool say = method == SayHash && !target.IsNone() && target.UserID == peerId;
            bool chat = method == ChatHash && target.IsNone();
            if (!say && !chat)
                return false;

            ZPackage parameters = packet.ReadPackage();
            if (chat)
                parameters.ReadVector3();
            int type = parameters.ReadInt();
            if (type < (int)Talker.Type.Whisper || type > (int)Talker.Type.Shout)
                return false;
            new UserInfo().Deserialize(ref parameters);
            return ReplayRequest.Matches(parameters.ReadString());
        }
        catch (IOException)
        {
            return false; // Malformed input is left to vanilla's receiver.
        }
        catch (ArgumentException)
        {
            return false;
        }
        finally
        {
            packet.SetPos(position);
        }
    }
}
