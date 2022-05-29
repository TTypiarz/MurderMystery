using CommandSystem;
using MurderMystery.Commands.Debug;
using System;

namespace MurderMystery.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class DebugCmds : ParentCommand
    {
        public DebugCmds() => LoadGeneratedCommands();

        public override string Command => "murdermysterydebug";
        public override string[] Aliases => new string[] { "mmd" };
        public override string Description => "Parent command for murder mystery (debug).";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new ShowRoles());
            RegisterCommand(new SetRole());
            RegisterCommand(new RoleIds());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Must provide a valid subcommand.";
            return false;
        }
    }
}
