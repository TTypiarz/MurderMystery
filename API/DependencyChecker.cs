using System;

namespace MurderMystery.API
{
    public static class DependencyChecker
    {
        public const string RespawnTimerQualifiedName = "RespawnTimer.RespawnTimer, RespawnTimer";

        public static bool CheckRespawnTimer()
        {
            if (Type.GetType(RespawnTimerQualifiedName) != null)
                return true;

            return false;
        }
    }
}
