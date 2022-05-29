using Exiled.Events.Extensions;
using HarmonyLib;
using MurderMystery.API.Internal;
using static Exiled.Events.Events;

namespace MurderMystery.Patches
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetRandomRoles))]
    public static class LateRoundStartPatch
    {
        public static event CustomEventHandler LateRoundStarted;

        private static void Postfix()
        {
            MMLog.Debug("Late round start patch called.");

            LateRoundStarted.InvokeSafely();
        }
    }
}
