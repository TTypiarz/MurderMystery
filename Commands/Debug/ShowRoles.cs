using CommandSystem;
using MurderMystery.API;
using MurderMystery.Extensions;
using System;
using System.Text;

namespace MurderMystery.Commands.Debug
{
    public class ShowRoles : ICommand
    {
        public string Command => "showroles";
        public string[] Aliases => new string[] { "showr" };
        public string Description => "Show all players and their roles.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("murdermystery.showroles", out response))
                return false;

            if (!MurderMystery.Singleton.WaitingPlayers)
            {
                response = "Murder mystery must be waiting for players to use this command.";
                return false;
            }

            StringBuilder builder = new StringBuilder().AppendLine("Roles:").Append("<size=20>");

            for (int i = 0; i < MMPlayer._list.Count; i++)
            {
                MMPlayer ply = MMPlayer._list[i];

                builder.AppendLine(string.Concat(
                    "(",
                    ply.Player.Id,
                    ") ",
                    ply.Player.Nickname, 
                    ": ",
                    ply.CustomRole?.ColoredName ?? "No role"
                ));
            }

            response = builder.Append("</size>").ToString();
            return true;
        }
    }
}
