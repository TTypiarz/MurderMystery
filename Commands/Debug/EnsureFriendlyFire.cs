using CommandSystem;
using MurderMystery.API.Enums;
using System;

namespace MurderMystery.Commands.Debug
{
    public class EnsureFriendlyFire : MMCommand
    {
        public override string Command => "ensureff";

        public override string[] Aliases => new string[0];

        public override string Description => "Ensures friendly fire is enabled during the event.";

        public override MMPerm Permission => MMPerm.Debug;

        public override bool PlayerOnly => false;

        public override bool ExecuteInternally(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!MurderMystery.Singleton.GamemodeManager.GamemodeEnabled)
            {
                response = "Murder Mystery must be active to use this command.";
                return false;
            }

            ServerConsole.FriendlyFire = true;
            response = "Friendly fire has been enabled.";
            return true;
        }
    }
}
