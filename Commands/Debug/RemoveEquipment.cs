using CommandSystem;
using Exiled.API.Features;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;
using MurderMystery.Extensions;
using System;

namespace MurderMystery.Commands.Debug
{
    public class RemoveEquipment : MMCommand
    {
        public override string Command => "removeequipment";

        public override string[] Aliases => new string[] { "removeeq", "req" };

        public override string Description => "Removes equipment from a specified players inventory.";

        public override MMPerm Permission => MMPerm.Debug;

        public override bool PlayerOnly => false;

        public override bool ExecuteInternally(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!MurderMystery.Singleton.GamemodeManager.GamemodeEnabled)
            {
                response = "Murder mystery must be active to use this command.";
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
                    player.RemoveEquipmentItems();
                    response = "Players inventory was cleared of equipment items.";
                    return false;
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
