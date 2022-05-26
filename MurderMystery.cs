using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;
using System;

namespace MurderMystery
{
    public class MurderMystery : Plugin<Config, Translation>
    {
        static MurderMystery()
        {
            Harmony = new Harmony("zereth.plugins.murdermystery");
            Harmony.PatchAll();
        }

        public override string Author => "Zereth";
        public override string Name => "MurderMystery";
        public override string Prefix => "murder_mystery";
        public override PluginPriority Priority => PluginPriority.Default;
        public override Version RequiredExiledVersion => new Version(5, 2, 1);
        public override Version Version => new Version(2, 0, 0);

        public static Harmony Harmony { get; }
        public static MurderMystery Singleton { get; private set; }

        public const bool InternalDebug = true;

        public override void OnEnabled()
        {
            Singleton = this;

            Config.Validate();
        }

        public override void OnDisabled()
        {
            Singleton = null;
        }

        public static bool AllowDebug()
        {
            return InternalDebug || (Singleton?.Config.Debug ?? false);
        }
    }
}
