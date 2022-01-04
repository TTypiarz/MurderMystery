using CommandSystem;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;
using System;

namespace MurderMystery.Commands.General
{
    public class ShowRoles : MMCommand
    {
        public override string Command => "showroles";

        public override string[] Aliases => new string[] { "showr", "sr" };

        public override string Description => "Shows all the players and their roles.";

        public override MMPerm Permission => MMPerm.ShowRoles;

        public override bool PlayerOnly => false;

        public override bool ExecuteInternally(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = string.Empty;

            for (int i = 0; i < MMPlayer.List.Count; i++)
            {
                MMPlayer ply = MMPlayer.List[i];

                response += $"({ply.Player.Id}) {ply.Player.Nickname}: {(ply.CustomRole == null ? "None" : ply.CustomRole.ColoredName)}\n";
            }

            return true;
        }
    }
}
