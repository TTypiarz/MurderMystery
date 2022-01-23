using Exiled.API.Features;
using MurderMystery.API.Features;

namespace MurderMystery.API
{
    internal static class DependencyUtilities
    {
        private static bool previousCommonUtilsCfg;

        internal static void HandleCedModV3(bool enable)
        {
            if (!DependencyChecker.CheckCedModV3())
                return;

            InternalHandleCedModV3(enable);
        }

        private static void InternalHandleCedModV3(bool enable)
        {
            CedMod.FriendlyFireAutoban.AdminDisabled = enable;
            MMLog.Debug($"{(enable ? "Disabled" : "Enabled")} CedMod autoban.");
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
                previousCommonUtilsCfg = Common_Utilities.Plugin.Singleton.Config.PlayerHealthInfo;
                Common_Utilities.Plugin.Singleton.Config.PlayerHealthInfo = false;
                MMLog.Debug("Disabled Common utilities player health info config.");
            }
            else
            {
                Common_Utilities.Plugin.Singleton.Config.PlayerHealthInfo = previousCommonUtilsCfg;
                MMLog.Debug("Reset Common utilities player health info config.");
            }
        }
    }
}
