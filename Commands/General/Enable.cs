using CommandSystem;
using MurderMystery.API.Enums;
using System;

namespace MurderMystery.Commands.General
{
    public class Enable : MMCommand
    {
        public override string Command => "enable";

        public override string[] Aliases => new string[] { "en" };

        public override string Description => "Enables the murder mystery gamemode.";

        public override MMPerm Permission => MMPerm.Enable;

        public override bool PlayerOnly => false;

        public override bool ExecuteInternally(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (MurderMystery.Singleton.GamemodeManager.GamemodeEnabled)
            {
                response = "Murder Mystery gamemode is currently active!";
                return false;
            }

            if (MurderMystery.Singleton.GamemodeManager.PrimaryEnabled)
            {
                response = "Murder Mystery gamemode is already enabled!";
                return false;
            }

            MurderMystery.Singleton.GamemodeManager.ToggleGamemode(true);
            response = "Murder Mystery gamemode enabled.";
            return true;
        }
    }
}
