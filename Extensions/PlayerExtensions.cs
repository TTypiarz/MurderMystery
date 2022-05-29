using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MurderMystery.API;
using System.Collections.Generic;

namespace MurderMystery.Extensions
{
    public static class PlayerExtensions
    {
        public static bool CheckPermission(this ICommandSender sender, string permission, out string response)
        {
            if (sender.CheckPermission(permission))
            {
                response = null;
                return true;
            }

            response = string.Concat("Missing permission: '", permission, "'");
            return false;
        }

        public static List<MMPlayer> ToMMPlayerList(this List<Player> players)
        {
            List<MMPlayer> result = new List<MMPlayer>();

            for (int i = 0; i < players.Count; i++)
                if (MMPlayer.TryGet(players[i], out MMPlayer player))
                    result.Add(player);

            return result;
        }

        public static List<MMPlayer> ToMMPlayerList(this List<ReferenceHub> hubs)
        {
            List<MMPlayer> result = new List<MMPlayer>();

            for (int i = 0; i < hubs.Count; i++)
                if (MMPlayer.TryGet(Player.Get(hubs[i]), out MMPlayer player))
                    result.Add(player);

            return result;
        }
    }
}
