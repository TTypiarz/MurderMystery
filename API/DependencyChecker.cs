using System;

namespace MurderMystery.API
{
    public static class DependencyChecker
    {
        public const string RespawnTimerQualifiedName = "RespawnTimer.RespawnTimer, RespawnTimer";

        public const string CedModV3QualifiedName = "CedMod.FriendlyFireAutoban, CedModV3";

        public static bool CheckRespawnTimer()
        {
            if (Type.GetType(RespawnTimerQualifiedName) != null)
                return true;

            return false;
        }

        public static bool CheckCedModV3()
        {
            if (Type.GetType(CedModV3QualifiedName) != null)
                return true;

            return false;
        }
    }
}
