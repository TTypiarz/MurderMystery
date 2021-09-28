using Exiled.API.Features;

namespace MurderMystery.API.Features
{
    public static class MMLog
    {
        public static void Info(object message)
        {
            Log.Info($"{MMUtilities.GetCallerString()} {message}");
        }
        public static void Debug(object message)
        {
            Log.Debug($"{MMUtilities.GetCallerString()} {message}", MurderMystery.Singleton != null && (MurderMystery.DebugVersion || MurderMystery.Singleton.Config.Debug));
        }
        public static void Warn(object message)
        {
            Log.Warn($"{MMUtilities.GetCallerString()} {message}");
        }
        public static void Error(object message)
        {
            Log.Error($"{MMUtilities.GetCallerString()} {message}");
        }
    }
}
