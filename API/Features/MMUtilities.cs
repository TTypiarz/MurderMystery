using Exiled.API.Features;
using MEC;
using System;
using System.Diagnostics;
using System.Reflection;

namespace MurderMystery.API.Features
{
    public static class MMUtilities
    {
        public static string GetCallerString()
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

        public static void ForceDisableGamemode(string message)
        {
            if (!MurderMystery.Singleton.GamemodeManager.PrimaryEnabled)
                return;

            Map.Broadcast(300, message, Broadcast.BroadcastFlags.Normal, true);
            Timing.CallDelayed(10, () =>
            {
                MurderMystery.Singleton.GamemodeManager.ToggleGamemode(false);
                Round.Restart(false);
            });
        }

        public static string GetInfoMsg()
        {
            Version vr = MurderMystery.Singleton.Version;

            return string.Concat(
                "Current version of Murder Mystery: <b>",
                vr.Major,
                ".",
                vr.Minor,
                ".",
                vr.Build,
                "</b>\nDebug version: ",
                MurderMystery.InternalDebugVersion,
                "\nDebug singleplayer: ",
                MurderMystery.InternalDebugSingleplayer,
#if DEBUG
                "\n[<color=#00ff00>TESTING RELEASE</color>]",
#endif
                "\n\nDeveloped by Zereth#1675\nGithub page: github.com/o5zereth/murdermystery\n- Post any issues with the plugin on the github."
                );
        }
    }
}
