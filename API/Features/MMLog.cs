using Exiled.API.Features;
using System;

namespace MurderMystery.API.Features
{
    public static class MMLog
    {
        public static void Info(object message)
        {
            Log.Info($"{MMUtilities.GetCallerString()} {message}");
#if DEBUG
            LogToDevelopers($"{MMUtilities.GetCallerString()}\n {message}", "cyan");
#endif
        }
        public static void Info(string caller, object message)
        {
            Log.Info($"{caller} {message}");
#if DEBUG
            LogToDevelopers($"{caller}\n {message}", "cyan");
#endif
        }
        public static void Debug(object message)
        {
            Log.Debug($"{MMUtilities.GetCallerString()} {message}", MurderMystery.InternalDebugVersion || (MurderMystery.Singleton != null && MurderMystery.Singleton.Config.Debug));
#if DEBUG
            LogToDevelopers($"{MMUtilities.GetCallerString()}\n {message}", "green");
#endif
        }
        public static void Debug(string caller, object message)
        {
            Log.Debug($"{caller} {message}", MurderMystery.InternalDebugVersion || (MurderMystery.Singleton != null && MurderMystery.Singleton.Config.Debug));
#if DEBUG
            LogToDevelopers($"{caller}\n {message}", "green");
#endif
        }
        public static void Warn(object message)
        {
            Log.Warn($"{MMUtilities.GetCallerString()} {message}");
#if DEBUG
            LogToDevelopers($"{MMUtilities.GetCallerString()}\n {message}", "magenta");
#endif
        }
        public static void Error(object message)
        {
            Log.Error($"{MMUtilities.GetCallerString()} {message}");
#if DEBUG
            LogToDevelopers($"{MMUtilities.GetCallerString()}\n {message}", "red");
#endif
        }
        public static void Error(Exception exception, object message)
        {
            Log.Error($"{MMUtilities.GetCallerString()} {message}\n{exception}");
#if DEBUG
            LogToDevelopers($"{MMUtilities.GetCallerString()}\n {message}\n{exception}", "red");
#endif
        }
        public static void Error(string caller, Exception exception, object message)
        {
            Log.Error($"{caller} {message}\n{exception}");
#if DEBUG
            LogToDevelopers($"{caller}\n {message}\n{exception}", "red");
#endif
        }

#if DEBUG
        internal static void LogToDevelopers(object message, string color = "white")
        {
            string log = "[MurderMystery] " + message.ToString();

            foreach (Player player in MurderMystery.Developers)
            {
                player.ReferenceHub.gameConsoleTransmission.SendToClient(player.Connection, log, color);
            }
        }
#endif
    }
}
