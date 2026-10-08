using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace Bedtime;

[BepInPlugin(PluginId, "Bedtime", PluginVersion)]
public sealed class Plugin : BaseUnityPlugin
{
    public const string PluginId = "augusdogus.mods.Bedtime";
    public const string PluginVersion = "1.1.0";

    private readonly BedAnnouncements _announcements = new();
    private readonly Harmony _harmony = new(PluginId);
    private readonly ReplayRequest _replayRequest = new();
    private bool _replayPending;
    private bool _showAwakePlayers = true;
    private static Plugin? _instance;

    private void Awake()
    {
        _instance = this;
        try
        {
            _showAwakePlayers = Config.Bind("Announcements", "ShowAwakePlayers", true,
                "Show the Not Sleeping list below the bed count. Disable for count-only announcements. Restart the server after changing this setting.").Value;
            _harmony.PatchAll(typeof(SleepUpdatePatch));
            _harmony.PatchAll(typeof(ChatCommandPatch));
            Logger.LogInfo("Bedtime loaded. Bed announcements run on dedicated servers; vanilla sleep rules are unchanged.");
        }
        catch (Exception error)
        {
            DisableAnnouncements(error);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
        _harmony.UnpatchSelf();
    }

    private void AnnounceBeds(bool sleeping)
    {
        try
        {
            bool repeat = _replayPending;
            _replayPending = false;
            ZNet network = ZNet.instance;
            if (network == null || !network.IsServer() || !network.IsDedicated() || sleeping)
            {
                _announcements.Clear();
                return;
            }

            ZRoutedRpc rpc = ZRoutedRpc.instance;
            if (rpc == null)
                return;

            // Use the same active characters and bed flag as vanilla's sleep check.
            List<ZDO> characters = network.GetAllCharacterZDOS();
            int inBed = 0;
            var awakePlayers = new List<string>();
            foreach (ZDO character in characters)
            {
                if (character.GetBool(ZDOVars.s_inBed))
                    inBed++;
                else
                    awakePlayers.Add(character.GetString(ZDOVars.s_playerName, "Unknown player"));
            }

            string? message = _announcements.Update(inBed, awakePlayers, _showAwakePlayers, repeat);
            if (message != null)
                rpc.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ShowMessage", (int)MessageHud.MessageType.TopLeft, message);
        }
        catch (Exception error)
        {
            DisableAnnouncements(error);
        }
    }

    private void DisableAnnouncements(Exception error)
    {
        _instance = null;
        Logger.LogError($"Bedtime could not initialize or announce bed states and has stopped announcements. " +
            $"Vanilla sleeping and world data are unchanged. Check mod/game compatibility and restart the server. {error}");
    }

    [HarmonyPatch(typeof(Game), "UpdateSleeping")]
    private static class SleepUpdatePatch
    {
        // Observe the existing server sleep pass before it can start the transition.
        // A void prefix neither skips the original nor changes its arguments or result.
        [HarmonyPrefix]
        private static void Prefix(bool ___m_sleeping)
        {
            _instance?.AnnounceBeds(___m_sleeping);
        }
    }

    [HarmonyPatch(typeof(ZRoutedRpc), "RPC_RoutedRPC")]
    private static class ChatCommandPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(ZRpc rpc, ZPackage pkg)
        {
            Plugin? plugin = _instance;
            ZNet network = ZNet.instance;
            if (plugin == null || network == null || !network.IsServer() || !network.IsDedicated())
                return true;
            foreach (ZNetPeer peer in network.GetPeers())
            {
                if (peer.m_rpc != rpc || !peer.IsReady())
                    continue;
                if (!ChatCommand.IsReplay(pkg, peer.m_uid))
                    return true;
                if (plugin._replayRequest.TryAccept(Time.realtimeSinceStartupAsDouble))
                    plugin._replayPending = true;
                // The requester already saw their local echo. Do not relay the
                // command text to other clients, including cooldown duplicates.
                return false;
            }
            return true;
        }
    }
}
