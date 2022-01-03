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

        public override string Description => "General murder mystery commands.";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new General.Enable());
            RegisterCommand(new General.Disable());
            RegisterCommand(new General.Status());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Invalid subcommand.";
            return false;
        }
    }
}
