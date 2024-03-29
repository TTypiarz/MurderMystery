﻿using CommandSystem;
using MurderMystery.Commands.General;
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
            RegisterCommand(new Enable());
            RegisterCommand(new Disable());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Must provide a valid subcommand.";
            return false;
        }
    }
}
