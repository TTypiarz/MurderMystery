using CommandSystem;
using Exiled.API.Features;
using MEC;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;
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
            if (!MurderMystery.Singleton.GamemodeManager.PrimaryEnabled)
            {
                response = "Murder Mystery gamemode is already disabled!";
                return false;
            }

            if (MurderMystery.Singleton.GamemodeManager.WaitingPlayers)
            {
                if (!arguments.Array.Contains("-f"))
                {
                    response = $"Murder Mystery gamemode is enabled for this round, and can't be disabled!\nUse disable -f to forcefully disable.";
                    return false;
                }
                else
                {
                    MMUtilities.ForceDisableGamemode("<size=30>Murder Mystery gamemode disabled. Round restart in 10 seconds.</size>");
                    response = "Murder Mystery gamemode disabled. Round restart in 10 seconds.";
                    return true;
                }
            }

            MurderMystery.Singleton.GamemodeManager.ToggleGamemode(false);
            response = "Murder Mystery gamemode disabled.";
            return true;
        }
    }
}
