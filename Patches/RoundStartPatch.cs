using HarmonyLib;
using MurderMystery.API.Enums;
using System;
using System.Reflection;

namespace MurderMystery.Patches
{
    public class RoundStartPatch
    {
        internal RoundStartPatch(Harmony harmony) => Harmony = harmony;
        private readonly Harmony Harmony;

        public MethodInfo OriginalMethod { get; } = typeof(CharacterClassManager).GetMethod(nameof(CharacterClassManager.SetRandomRoles), BindingFlags.NonPublic | BindingFlags.Instance);
        public HarmonyMethod PatchMethod { get; } = new HarmonyMethod(typeof(RoundStartPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static));

        public bool Patched { get; private set; } = false;

        internal void Patch(bool patch)
        {
            MurderMystery.Debug($"{(patch ? "Enabling" : "Disabling")} patch.");

            try
            {
                if (patch ^ Patched)
                {
                    if (patch)
                    {
                        Harmony.Patch(OriginalMethod, PatchMethod);
                    }
                    else
                    {
                        Harmony.Unpatch(OriginalMethod, PatchMethod.method);
                    }

                    Patched = patch;
                }
            }
            catch (Exception e)
            {
                MurderMystery.Error($"ERROR:\n{e}");
            }
        }

        private static void Prefix()
        {
            MurderMystery.Debug("Patch called.");

            MurderMystery.Singleton.EventHandlers.ToggleEvent(MMEventType.Gamemode, true);

            MurderMystery.Singleton.RoundStartPatch.Patch(false);
        }
    }
}
