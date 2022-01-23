using HarmonyLib;
using System.Reflection;

namespace MurderMystery.Patches
{
    public class AchievementNullRefPatch
    {
        public static MethodInfo Original { get; } = typeof(Achievements.Handlers.ItemPickupHandler).GetMethod("OnItemAdded", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        public static HarmonyMethod Patch { get; } = new HarmonyMethod(typeof(AchievementNullRefPatch).GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic));

        private static bool Prefix()
        {
            return !MurderMystery.Singleton.GamemodeManager.GamemodeEnabled;
        }
    }
}
