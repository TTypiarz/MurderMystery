﻿using HarmonyLib;

namespace MurderMystery.Patches
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetRandomRoles))]
    public class LateRoundStartPatch
    {
        private static void Postfix()
        {
            if (MurderMystery.Singleton.GamemodeManager.PrimaryEnabled)
            {
                MurderMystery.Singleton.GamemodeManager.LateStartGamemode();
            }
        }
    }
}