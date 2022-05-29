using CommandSystem;
using MurderMystery.API;
using MurderMystery.API.Enums;
using MurderMystery.Extensions;
using System;
using System.Collections.Generic;
using Utils;

namespace MurderMystery.Commands.Debug
{
    public class SetRole : ICommand
    {
        public string Command => "setrole";
        public string[] Aliases => new string[] { "setr" };
        public string Description => "Sets the specified player(s) roles.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("murdermystery.setroles", out response))
                return false;

            if (!MurderMystery.InternalDebug && !MurderMystery.Singleton.Started)
            {
                response = "Setting roles before the round is started is only enabled in debug builds.";
                return false;
            }

            string[] args = arguments.Array;

            if (args.Length < 4)
            {
                response = "You must provide 2 arguments!";
                return false;
            }

            List<MMPlayer> players = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out _).ToMMPlayerList();

            if (players.Count == 0)
            {
                response = "The command did not affect any players. Check your syntax.";
                return false;
            }

            if (!int.TryParse(args[3], out int roleId))
            {
                response = "Second argument must be an integer representing the role id.";
                return false;
            }

            if (!Enum.IsDefined(typeof(MMRole), roleId))
            {
                response = "That role id is not defined!\nUse command 'roleids' to see a list of role ids.";
                return false;
            }

            MMRole newRole = (MMRole)roleId;

            if (!MurderMystery.InternalDebug && newRole == MMRole.None)
            {
                response = "Setting role to 'None' is only enabled for debug builds.";
                return false;
            }

            int success = 0;
            for (int i = 0; i < players.Count; i++)
            {
                MMPlayer ply = players[i];
                MMCustomRole oldRoleInstance = ply.CustomRole;

                ply.Role = newRole;

                if (oldRoleInstance != ply.CustomRole)
                    success++;
            }

            if (success == 0)
            {
                response = "The command did not affect any players. (Roles may have not changed)";
                return false;
            }

            response = string.Concat("Success! The request affected ", success, " player(s). (Set to ", ((MMRole)roleId).ToString(), ")");
            return true;
        }
    }
}
