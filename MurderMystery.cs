using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using HarmonyLib;
using MurderMystery.API.Features;
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
        public static bool DebugVersion => InternalDebugVersion;
        internal const bool InternalDebugVersion = true;

        public GamemodeManager GamemodeManager { get; private set; }
        public Harmony Harmony { get; private set; }

        public override void OnEnabled()
        {
            Singleton = this;
            GamemodeManager = new GamemodeManager();
            Harmony = new Harmony("zereth.plugins.murdermystery");

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

            base.OnDisabled();
        }
    }
}
