using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;

namespace Bedtime;

[BepInPlugin(PluginId, "Bedtime", PluginVersion)]
public sealed class Plugin : BaseUnityPlugin
{
    public const string PluginId = "augusdogus.mods.Bedtime";
    public const string PluginVersion = "1.0.0";

    private readonly BedAnnouncements _announcements = new();
    private readonly Harmony _harmony = new(PluginId);
    private static Plugin? _instance;

    private void Awake()
    {
        _instance = this;
        try
        {
            _harmony.PatchAll(typeof(SleepUpdatePatch));
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
            foreach (ZDO character in characters)
            {
                if (character.GetBool(ZDOVars.s_inBed))
                    inBed++;
            }

            string? message = _announcements.Update(inBed, characters.Count);
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
}
