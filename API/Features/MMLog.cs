﻿using Exiled.API.Features;
using System;

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
            Log.Debug($"{MMUtilities.GetCallerString()} {message}", MurderMystery.InternalDebugVersion || (MurderMystery.Singleton != null && MurderMystery.Singleton.Config.Debug));
        }
        public static void Debug(string caller, object message)
        {
            Log.Debug($"{caller} {message}", MurderMystery.InternalDebugVersion || (MurderMystery.Singleton != null && MurderMystery.Singleton.Config.Debug));
        }
        public static void Warn(object message)
        {
            Log.Warn($"{MMUtilities.GetCallerString()} {message}");
        }
        public static void Error(object message)
        {
            Log.Error($"{MMUtilities.GetCallerString()} {message}");
        }
        public static void Error(Exception exception, object message)
        {
            Log.Error($"{MMUtilities.GetCallerString()} {message}\n{exception}");
        }
    }
}
