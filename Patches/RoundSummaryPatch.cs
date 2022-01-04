using HarmonyLib;
using System.Reflection;

namespace MurderMystery.Patches
{
    public class RoundSummaryPatch
    {
        public static MethodInfo Original { get; } = typeof(RoundSummary).GetMethod("RpcShowRoundSummary", BindingFlags.NonPublic | BindingFlags.Instance);
        public static HarmonyMethod Patch { get; } = new HarmonyMethod(typeof(RoundSummaryPatch).GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Static));

        private static bool Prefix()
        {
            if (MurderMystery.Singleton.GamemodeManager.Started)
                return false;

            return true;
        }
    }
}
