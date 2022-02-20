using System;

namespace MurderMystery.API
{
    public static class DependencyChecker
    {
        public const string RespawnTimerQualifiedName = "RespawnTimer.EventHandler, RespawnTimer";

        public const string CedModV3QualifiedName = "CedMod.FriendlyFireAutoban, CedModV3";

        public const string CommonUtilsQualifiedName = "Common_Utilities.Config, Common_Utilities";

        public static bool CheckRespawnTimer()
        {
            return Type.GetType(RespawnTimerQualifiedName) != null;
        }

        public static bool CheckCedModV3()
        {
            return Type.GetType(CedModV3QualifiedName) != null;
        }

        public static bool CheckCommonUtils()
        {
            return Type.GetType(CommonUtilsQualifiedName) != null;
        }
    }
}
