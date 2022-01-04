using HarmonyLib;
using System.Reflection;

namespace MurderMystery.Patches
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetRandomRoles))]
    public class LateRoundStartPatch
    {
        public static MethodInfo Original { get; } = typeof(CharacterClassManager).GetMethod("SetRandomRoles", BindingFlags.NonPublic | BindingFlags.Instance);
        public static HarmonyMethod Patch { get; } = new HarmonyMethod(typeof(LateRoundStartPatch).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static));

        private static void Postfix()
        {
            MurderMystery.Singleton.GamemodeManager.LateStartGamemode();
        }
    }
}
