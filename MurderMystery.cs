using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using HarmonyLib;
using MEC;
using MurderMystery.API;
using MurderMystery.API.Features;
using MurderMystery.Patches;
using System;
using System.Collections.Generic;

namespace MurderMystery
{
    public class MurderMystery : Plugin<Config>
    {
        public override string Author => "Zereth";
        public override string Name => "MurderMystery";
        public override string Prefix => "murder_mystery";
        public override PluginPriority Priority => PluginPriority.Default;
        public override Version RequiredExiledVersion => new Version(5, 0, 0);
        public override Version Version => new Version(1, 3, 2);

        public static MurderMystery Singleton { get; private set; }
        public static bool DebugVersion => InternalDebugVersion;
        internal const bool InternalDebugVersion = false;
        internal const bool InternalDebugSingleplayer = false;

        public static ItemType[] AllowedItems => new ItemType[]
        {
            ItemType.Painkillers,
            ItemType.Medkit,
            ItemType.Adrenaline,
            ItemType.ArmorLight,
            ItemType.ArmorHeavy,
            ItemType.Radio,
            ItemType.Flashlight,
            ItemType.Coin,
            ItemType.SCP500,
            ItemType.SCP2176,
            ItemType.GrenadeFlash
        };

        public static string[] GamemodeInformation => new string[]
        {
            "\n<size=80><b>Murder Mystery</b></size>",
            "",
            "Welcome to murder mystery!",
            "In this gamemode there are three roles:",
            "<color=#00ff00>Innocent</color>, <color=#ff0000>Murderer</color>, and <color=#0000ff>Detective</color>.",
            "",
            "<size=35><b><color=#00ff00>Innocent</color>:</b></size>",
            "Must survive, avoid <color=#ff0000>murderers</color>, and relay information to <color=#0000ff>detectives</color>.",
            "Only spawns with painkillers, but can loot items around the map.",
            "If you find a <b>revolver</b>, upon picking it up you will become a <color=#0000ff>detective</color>.",
            "",
            "<size=35><b><color=#ff0000>Murderer</color>:</b></size>",
            "Must <color=#ff0000>kill all</color> <color=#00ff00>innocents</color> and <color=#0000ff>detectives</color>",
            "Spawns with a COM-18 pistol, SCP-268, and painkillers.",
            "Fellow murderers can be seen by looking at them.",
            "",
            "<size=35><b><color=#0000ff>Detective</color>:</b></size>",
            "Must protect <color=#00ff00>innocents</color>, and kill <color=#ff0000>murderers</color>",
            "Spawns with a Revolver, medkit, and painkillers.",
            "Fellow detectives can be seen by looking at them.",
            "\n",
            "<size=35><b>Game Mechanics:</b></size>",
            "<size=30><b>Round Timer (Config)</b></size>",
            "- This is a mechanic in which <color=#00ff00>innocents win by default</color> after a set amount of time, if enabled.",
            "\n",
            "<size=30><b>Murderer Generators (Config)</b></size>",
            "- This is a mechanic in which <color=#ff0000>murderers win by default</color> if all generators are activated.",
            "- Generators are unlocked after a set amount of time, if enabled.",
            "\n",
            "<size=30><b>Murderer 939 Vision (Config)</b></size>",
            "- This is a mechanic in which <color=#ff0000>murderers see players through walls</color>.",
            "- Murderers are given this ability after a set amount of time, if enabled.",
            "- <b>STANDING STILL DOES NOT AFFECT THE VISION, THEY CAN STILL SEE YOU.</b>",
            "\n",
            "<size=30><b>Passive mechanics:</b></size>",
            "- Keycard lockers open when the round starts, so you can access allowed items.",
            "- Flashbangs affect everyone except the person that throws them. Teams are not accounted for.",
            //"- <color=#0000ff>Detectives</color> do <b>1.2x the amount of damage</b> for balancing reasons.",
            "- You are not allowed to access any area other than heavy containment.",
            "\n",
            "<size=40><b>Development information:</b></size>",
            MMUtilities.GetInfoMsg()
        };

#if DEBUG
        public static List<string> DeveloperIds => new List<string>()
        {
            "76561198288227848@steam" // Zereth
        };

        public static List<Player> Developers { get; } = new List<Player>();
#endif

        public GamemodeManager GamemodeManager { get; private set; }
        public Harmony Harmony { get; private set; }

        public override void OnEnabled()
        {
            Singleton = this;
            GamemodeManager = new GamemodeManager();
            Harmony = new Harmony("zereth.plugins.murdermystery");

            Harmony.Patch(LateRoundStartPatch.Original, null, LateRoundStartPatch.Patch);
            Harmony.Patch(RoundSummaryPatch.Original, RoundSummaryPatch.Patch);
            Harmony.Patch(AchievementNullRefPatch.Original, AchievementNullRefPatch.Patch);

            Timing.CallDelayed(3f, () => // Issues patching dependiencies before all plugins load.
            {
                try
                {
                    if (DependencyChecker.CheckRespawnTimer())
                    {
                        RespawnTimerPatch.EnablePatch();
                        MMLog.Info("[MurderMystery::OnEnabled]", "Patched respawn timer.");
                    }
                }
                catch (Exception e)
                {
                    MMLog.Error("[MurderMystery::OnEnabled]", e, "Failed to patch respawn timer.");
                }
            });

#if DEBUG
            Exiled.Events.Handlers.Player.Verified += VerifiedDeveloperCheck;
            Exiled.Events.Handlers.Player.Destroying += DestroyingDeveloperCheck;
            Exiled.Events.Handlers.Server.RestartingRound += RestartingDeveloperCheck;
#endif

            Config.Validate();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            GamemodeManager.ToggleGamemode(false);

            Harmony.UnpatchAll();
            Harmony = null;
            GamemodeManager = null;
            Singleton = null;

#if DEBUG
            Exiled.Events.Handlers.Player.Verified -= VerifiedDeveloperCheck;
            Exiled.Events.Handlers.Player.Destroying -= DestroyingDeveloperCheck;
            Exiled.Events.Handlers.Server.RestartingRound -= RestartingDeveloperCheck;
#endif

            base.OnDisabled();
        }

#if DEBUG
        private void VerifiedDeveloperCheck(VerifiedEventArgs ev)
        {
            if (DeveloperIds.Contains(ev.Player.UserId))
            {
                Developers.Add(ev.Player);
            }
        }

        private void DestroyingDeveloperCheck(DestroyingEventArgs ev)
        {
            Developers.Remove(ev.Player);
        }

        private void RestartingDeveloperCheck()
        {
            Developers.Clear();
        }
#endif
    }
}
