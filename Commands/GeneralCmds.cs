using CommandSystem;
using System;

namespace MurderMystery.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class GeneralCmds : ParentCommand
    {
        public GeneralCmds() => LoadGeneratedCommands();

        public override string Command => "murdermystery";

        public override string[] Aliases => new string[] { "mm" };

        public override string Description => "Parent command for murder mystery.";

        public override void LoadGeneratedCommands()
        {
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            // placeholder for testing.

            if (MurderMystery.Singleton != null)
            {
                MurderMystery.Singleton.ToggleGamemode(true);

                response = "Enabled murder mystery.";
                return true;
            }

            response = "Murder mystery singleton is null.";
            return false;
        }
    }
}
