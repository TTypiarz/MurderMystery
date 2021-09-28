using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;
using MurderMystery.Patches;
using System;

namespace MurderMystery
{
    public class MurderMystery : Plugin<Config>
    {
        public override string Author => "Zereth";
        public override string Name => "MurderMystery";
        public override string Prefix => "murder_mystery";
        public override PluginPriority Priority => PluginPriority.Default;
        public override Version RequiredExiledVersion => new Version(3, 0, 0);
        public override Version Version => new Version(1, 0, 0);

        public static MurderMystery Singleton { get; private set; }
        public static bool DebugVersion => true;

        public EventHandlers EventHandlers { get; private set; }
        public Harmony Harmony { get; private set; }

        public RoundStartPatch RoundStartPatch { get; private set; }

        public override void OnEnabled()
        {
            Singleton = this;
            EventHandlers = new EventHandlers(this);
            Harmony = new Harmony("zereth.plugins.murdermystery");

            RoundStartPatch = new RoundStartPatch(Harmony);

            Config.Validate();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            ToggleGamemode(false);

            RoundStartPatch = null;

            Harmony.UnpatchAll();
            Harmony = null;
            EventHandlers = null;
            Singleton = null;

            base.OnDisabled();
        }

        #region GamemodeManager
        internal void ToggleGamemode(bool enable)
        {
            if (enable ^ EventHandlers.PrimaryEnabled)
            {
                MMLog.Debug($"{(enable ? "Enabling" : "Disabling")} the murder mystery gamemode.");

                if (enable)
                {
                    EventHandlers.ToggleEvent(MMEventType.Primary, true);
                }
                else
                {
                    EventHandlers.ToggleEvent(MMEventType.Primary, false);
                    EventHandlers.ToggleEvent(MMEventType.Player, false);
                    EventHandlers.ToggleEvent(MMEventType.Gamemode, false);

                    RoundStartPatch.Patch(false);

                    EventHandlers.Started = false;
                }
            }
            else
            {
                MMLog.Debug($"\nCall invalid: {(enable ? "Enabling" : "Disabling")}\nCaller: {MMUtilities.GetCallerString()}");
            }
        }

        internal void StartGamemode()
        {
            try
            {
                MMLog.Debug("Primary function called.");

                // Event will be setup here.
            }
            catch (Exception e)
            {
                MMLog.Error($"FATAL ERROR:\n{e}");
            }
        }
        #endregion
    }
}
