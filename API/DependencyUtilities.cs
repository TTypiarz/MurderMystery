using Exiled.API.Features;
using MEC;
using MurderMystery.API.Features;

namespace MurderMystery.API
{
    internal static class DependencyUtilities
    {
        private static bool previousHealthInfoCfg;
        private static float previousAutoNukeCfg;
        private static float previousItemCleanupCfg;

        internal static void HandleCedModV3(bool enable)
        {
            if (!DependencyChecker.CheckCedModV3())
                return;

            InternalHandleCedModV3(enable);
        }

        private static void InternalHandleCedModV3(bool enable)
        {
            CedMod.FriendlyFireAutoban.AdminDisabled = enable;
            MMLog.Info($"{(enable ? "Disabled" : "Enabled")} CedMod autoban.");
        }

        internal static void HandleCommonUtils(bool enable)
        {
            if (!DependencyChecker.CheckCommonUtils())
                return;

            InternalHandleCommonUtils(enable);
        }

        private static void InternalHandleCommonUtils(bool enable)
        {
            if (enable)
            {
                previousHealthInfoCfg = Common_Utilities.Plugin.Singleton.Config.PlayerHealthInfo;
                Common_Utilities.Plugin.Singleton.Config.PlayerHealthInfo = false;
                MMLog.Info("Disabled Common utilities player health info config.");

                previousAutoNukeCfg = Common_Utilities.Plugin.Singleton.Config.AutonukeTime;
                Common_Utilities.Plugin.Singleton.Config.AutonukeTime = -1f;
                MMLog.Info("Disabled Common utilities auto nuke config.");

                previousItemCleanupCfg = Common_Utilities.Plugin.Singleton.Config.ItemCleanupDelay;
                Common_Utilities.Plugin.Singleton.Config.ItemCleanupDelay = 0f;
                MMLog.Info("Disabled Common utilities item cleanup config.");
            }
            else
            {
                Common_Utilities.Plugin.Singleton.Config.PlayerHealthInfo = previousHealthInfoCfg;
                Common_Utilities.Plugin.Singleton.Config.AutonukeTime = previousAutoNukeCfg;
                Common_Utilities.Plugin.Singleton.Config.ItemCleanupDelay = previousItemCleanupCfg;
                MMLog.Info("Reset Common utilities configs.");
            }
        }
    }
}
