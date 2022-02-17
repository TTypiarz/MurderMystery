using CommandSystem;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;
using System;

namespace MurderMystery.Commands.Debug
{
    public class Info : MMCommand
    {
        public override string Command => "info";

        public override string[] Aliases => new string[] { "inf" };

        public override string Description => "Shows info about the current version of murder mystery.";

        public override MMPerm Permission => MMPerm.None;

        public override bool PlayerOnly => false;

        public override bool ExecuteInternally(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = MMUtilities.GetInfoMsg();
            return true;
        }
    }
}
