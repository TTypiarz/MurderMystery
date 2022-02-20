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
        public override Version RequiredExiledVersion => new Version(4, 2, 3);
        public override Version Version => new Version(1, 3, 0);

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
