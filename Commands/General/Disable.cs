using CommandSystem;
using MurderMystery.API.Enums;
using System;

namespace MurderMystery.Commands.General
{
    public class Disable : MMCommand
    {
        public override string Command => "disable";

        public override string[] Aliases => new string[] { "dis" };

        public override string Description => "Disables the murder mystery gamemode.";

        public override MMPerm Permission => MMPerm.Enable;

        public override bool PlayerOnly => false;

        public override bool ExecuteInternally(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (MurderMystery.Singleton.GamemodeManager.GamemodeEnabled)
            {
                response = "Murder Mystery gamemode is currently active, and can't be disabled!";
                return false;
            }

            if (!MurderMystery.Singleton.GamemodeManager.PrimaryEnabled)
            {
                response = "Murder Mystery gamemode is already disabled!";
                return false;
            }

            MurderMystery.Singleton.GamemodeManager.ToggleGamemode(false);
            response = "Murder Mystery gamemode disabled.";
            return true;
        }
    }
}
