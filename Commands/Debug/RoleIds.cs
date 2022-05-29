using CommandSystem;
using MurderMystery.API.Enums;
using System;
using System.Text;

namespace MurderMystery.Commands.Debug
{
    public class RoleIds : ICommand
    {
        public string Command => "roleids";
        public string[] Aliases => new string[] { "rids", "showids", "sids" };
        public string Description => "Shows a list of role ids for the gamemode.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            int[] ids = Enum.GetValues(typeof(MMRole)) as int[];

            StringBuilder builder = new StringBuilder().AppendLine("Role ids:");

            for (int i = 0; i < ids.Length; i++)
            {
                builder.AppendLine(string.Concat((
                    (MMRole)ids[i]).ToString(),
                    ": ",
                    ids[i]
                ));
            }

            response = builder.ToString();
            return true;
        }
    }
}
