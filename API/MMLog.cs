using Exiled.API.Features;
using System.Diagnostics;
using System.Reflection;

namespace MurderMystery.API
{
    internal static class MMLog
    {
        public static void Info(string message)
        {
            Log.Info(string.Concat(GetCaller(), " ", message));
        }
        public static void Info(object message)
        {
            Log.Info(string.Concat(GetCaller(), " ", message.ToString()));
        }

        public static void Debug(string message)
        {
            Log.Debug(string.Concat(GetCaller(), " ", message), MurderMystery.AllowDebug());
        }
        public static void Debug(object message)
        {
            Log.Debug(string.Concat(GetCaller(), " ", message.ToString()), MurderMystery.AllowDebug());
        }

        public static void Warn(string message)
        {
            Log.Warn(string.Concat(GetCaller(), " ", message));
        }
        public static void Warn(object message)
        {
            Log.Warn(string.Concat(GetCaller(), " ", message.ToString()));
        }

        public static void Error(string message)
        {
            Log.Error(string.Concat(GetCaller(), " ", message));
        }
        public static void Error(object message)
        {
            Log.Error(string.Concat(GetCaller(), " ", message.ToString()));
        }

        private static string GetCaller()
        {
            try
            {
                MethodBase method = new StackFrame(2).GetMethod();

                return $"[{method.DeclaringType.Name}::{method.Name}]";
            }
            catch
            {
                return "[null]";
            }
        }
    }
}
