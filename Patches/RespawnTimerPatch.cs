using HarmonyLib;
using MEC;
using MurderMystery.API;
using MurderMystery.API.Features;
using System;
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
                try
                {
                    Timing.KillCoroutines(RespawnTimer.EventHandler.timerCoroutine);
                    RespawnTimer.EventHandler.timerCoroutine = default;
                    MMLog.Info("Killed RespawnTimer coroutine.");
                }
                catch (Exception e)
                {
                    MMLog.Error(e, "Killing respawn timer coroutine failed!");
                }
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
            try
            {
                MethodInfo original = typeof(RespawnTimer.EventHandler).GetMethod("OnRoundStart", BindingFlags.Static | BindingFlags.NonPublic);
                HarmonyMethod patch = new HarmonyMethod(typeof(RespawnTimerPatch).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic));
                MurderMystery.Singleton.Harmony.Patch(original, null, patch);

                PatchEnabled = true;
            }
            catch (Exception e)
            {
                MMLog.Error(e, "Patching respawn timer failed! You may need to update.");
            }
        }
    }
}
