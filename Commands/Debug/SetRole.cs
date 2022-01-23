using CommandSystem;
using Exiled.API.Features;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;
using System;

namespace MurderMystery.Commands.Debug
{
    public class SetRole : MMCommand
    {
        public override string Command => "setrole";

        public override string[] Aliases => new string[] { "sr" };

        public override string Description => "Set a players role during murder mystery.";

        public override MMPerm Permission => MMPerm.SetRoles;

        public override bool PlayerOnly => false;

        public override bool ExecuteInternally(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string[] args = arguments.Array;

            if (args.Length < 4)
            {
                response = "You must provide 2 arguments to this command.";
                return false;
            }

            if (int.TryParse(args[2], out int playerid))
            {
                if (MMPlayer.Get(Player.Get(playerid), out MMPlayer player))
                {
                    switch (args[3].ToLower())
                    {
                        case "i":
                        case "innocent":
                            player.Role = MMRole.Innocent;
                            response = "Players role was set to innocent.";
                            return true;
                        case "m":
                        case "murderer":
                            player.Role = MMRole.Murderer;
                            response = "Players role was set to murderer.";
                            return true;
                        case "d":
                        case "detective":
                            player.Role = MMRole.Detective;
                            response = "Players role was set to detective.";
                            return true;
                        case "s":
                        case "spectator":
                            player.Role = MMRole.Spectator;
                            response = "Players role was set to spectator.";
                            return true;

                        default:
                            response = $"Role named: {args[3]} was not found.";
                            return false;
                    }
                }
                else
                {
                    response = "Player with the id specified was not found.";
                    return false;
                }
            }
            else
            {
                response = "Parsing player id failed.";
                return false;
            }
        }
    }
}
