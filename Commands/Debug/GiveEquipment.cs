using CommandSystem;
using Exiled.API.Features;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;
using MurderMystery.API.Interfaces;
using System;

namespace MurderMystery.Commands.Debug
{
    public class GiveEquipment : MMCommand
    {
        public override string Command => "giveequipment";

        public override string[] Aliases => new string[] { "giveeq", "geq" };

        public override string Description => "Forcefully calls the give equipment method on the specified player.";

        public override MMPerm Permission => MMPerm.Debug;

        public override bool PlayerOnly => false;

        public override bool ExecuteInternally(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!MurderMystery.Singleton.GamemodeManager.GamemodeEnabled)
            {
                response = "Murder Mystery must be active to use this command.";
                return false;
            }
            
            string[] args = arguments.Array;

            if (args.Length < 3)
            {
                response = "You must supply a player id.";
                return false;
            }

            if (int.TryParse(args[2], out int playerid))
            {
                if (MMPlayer.Get(Player.Get(playerid), out MMPlayer player))
                {
                    if (player.CustomRole == null)
                    {
                        response = "That player does not have a role.";
                        return false;
                    }

                    if (player.CustomRole is IEquipment equipment)
                    {
                        try
                        {
                            equipment.GiveEquipment(player);
                            player.Player.Broadcast(10, equipment.EquipmentMessage);
                            response = "Successfully forced equipment on the player.";
                            return true;
                        }
                        catch (Exception e)
                        {
                            response = "\nAn error occured:\n" + e;
                            return false;
                        }
                    }
                    else
                    {
                        response = $"The player's role: ({player.CustomRole.ColoredName}) does not have equipment.";
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
