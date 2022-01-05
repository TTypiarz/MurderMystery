using HarmonyLib;
using MEC;
using MurderMystery.API;
using MurderMystery.API.Features;
using System.Reflection;

namespace MurderMystery.Patches
{
    public class RespawnTimerPatch
    {
        public static bool PatchEnabled { get; private set; } = false;

        private static void Postfix()
        {
            if (MurderMystery.Singleton.GamemodeManager.WaitingPlayers)
            {
                Timing.KillCoroutines(RespawnTimer.RespawnTimer.Singleton.handler.timerCoroutine);
                RespawnTimer.RespawnTimer.Singleton.handler.timerCoroutine = default;
                MMLog.Info("Killed RespawnTimer coroutine.");
            }
        }

        internal static void TogglePatch(bool enable)
        {
            MMLog.Debug($"TogglePatch called by: {MMUtilities.GetCallerString()}");

            if (!DependencyChecker.CheckRespawnTimer())
                return;

            InternalTogglePatch(enable);
        }

        private static void InternalTogglePatch(bool enable)
        {
            MethodInfo original = typeof(RespawnTimer.Handler).GetMethod("OnRoundStart", BindingFlags.Instance | BindingFlags.NonPublic);

            if (enable)
            {
                HarmonyMethod patch = new HarmonyMethod(typeof(RespawnTimerPatch).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic));
                MurderMystery.Singleton.Harmony.Patch(original, null, patch);

                PatchEnabled = true;
            }
            else
            {
                MurderMystery.Singleton.Harmony.Unpatch(original, HarmonyPatchType.Postfix);

                PatchEnabled = false;
            }
        }
    }
}
