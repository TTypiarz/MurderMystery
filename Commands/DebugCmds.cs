using CommandSystem;
using System;

namespace MurderMystery.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DebugCmds : ParentCommand
    {
        public DebugCmds() => LoadGeneratedCommands();

        public override string Command => "mmd";

        public override string[] Aliases => new string[] { "murdermyserydebug", "mmdebug" };

        public override string Description => "Debug commands for murder mystery.";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Debug.Status());
            RegisterCommand(new Debug.GiveEquipment());
            RegisterCommand(new Debug.Info());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Invalid subcommand.";
            return false;
        }
    }
}
