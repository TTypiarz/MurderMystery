using Exiled.API.Features;
using MurderMystery.API.Features;

namespace MurderMystery.API
{
    internal static class DependencyUtilities
    {
        public static void HandleCedModV3(bool enable)
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
    }
}
