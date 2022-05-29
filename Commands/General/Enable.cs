using CommandSystem;
using MurderMystery.Extensions;
using System;

namespace MurderMystery.Commands.General
{
    public class Enable : ICommand
    {
        public string Command => "enable";
        public string[] Aliases => new string[] { "en" };
        public string Description => "Enables murder mystery for the next round.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("murdermystery.enable", out response))
                return false;

            if (MurderMystery.Singleton.GamemodeHandlers.Enabled)
            {
                response = "Murder Mystery gamemode is currently active!";
                return false;
            }

            if (MurderMystery.Singleton.PrimaryHandlers.Enabled)
            {
                response = "Murder Mystery gamemode is already enabled!";
                return false;
            }

            MurderMystery.Singleton.ToggleGamemode(true);
            response = "Murder Mystery gamemode has been enabled for the next round restart.";
            return true;
        }
    }
}
