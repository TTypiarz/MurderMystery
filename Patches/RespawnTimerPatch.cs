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

        internal static void EnablePatch()
        {
            if (!DependencyChecker.CheckRespawnTimer())
                return;

            InternalEnablePatch();
        }

        private static void InternalEnablePatch()
        {
            MethodInfo original = typeof(RespawnTimer.Handler).GetMethod("OnRoundStart", BindingFlags.Instance | BindingFlags.NonPublic);
            HarmonyMethod patch = new HarmonyMethod(typeof(RespawnTimerPatch).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic));
            MurderMystery.Singleton.Harmony.Patch(original, null, patch);

            PatchEnabled = true;
        }
    }
}
