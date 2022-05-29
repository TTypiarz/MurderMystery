using CommandSystem;
using MurderMystery.Extensions;
using System;

namespace MurderMystery.Commands.General
{
    public class Disable : ICommand
    {
        public string Command => "disable";
        public string[] Aliases => new string[] { "dis" };
        public string Description => "Disables murder mystery.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("murdermystery.enable", out response))
                return false;

            if (MurderMystery.Singleton.WaitingPlayers)
            {
                if (!arguments.Array.Contains("-f"))
                {
                    response = $"Murder Mystery gamemode is enabled for this round, and can't be disabled!\nUse disable -f to forcefully disable.";
                    return false;
                }
                else
                {
                    MurderMystery.Singleton.DisableAndRestartWithMessage("<size=30>Murder Mystery gamemode disabled by an admin. Round restart in 10 seconds.</size>");
                    response = "Murder Mystery gamemode disabled. Round restart in 10 seconds.";
                    return true;
                }
            }

            MurderMystery.Singleton.ToggleGamemode(false);
            response = "Murder Mystery gamemode disabled.";
            return true;
        }
    }
}
