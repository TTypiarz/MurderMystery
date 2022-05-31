using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MurderMystery.API;
using MurderMystery.API.Enums;
using System;
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

        public static List<MMPlayer> GetRole(this List<MMPlayer> players, MMRole role)
        {
            List<MMPlayer> result = new List<MMPlayer>();

            if (players.Count == 0) { return result; }

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Role == role)
                {
                    result.Add(players[i]);
                }
            }

            return result;
        }

        public static int GetRoleCount(this List<MMPlayer> players, MMRole role)
        {
            int result = 0;

            if (players.Count == 0) { return result; }

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Role == role)
                {
                    result++;
                }
            }

            return result;
        }

        public static List<MMPlayer> GetRoles(this List<MMPlayer> players, params MMRole[] roles)
        {
            List<MMPlayer> result = new List<MMPlayer>();

            if (Math.Min(players.Count, roles.Length) == 0) { return result; }

            for (int ply = 0; ply < players.Count; ply++)
            {
                for (int role = 0; role < roles.Length; role++)
                {
                    if (players[ply].Role == roles[role])
                    {
                        result.Add(players[ply]);
                        break;
                    }
                }
            }

            return result;
        }

        public static int GetRolesCount(this List<MMPlayer> players, params MMRole[] roles)
        {
            int result = 0;

            if (Math.Min(players.Count, roles.Length) == 0) { return result; }

            for (int ply = 0; ply < players.Count; ply++)
            {
                for (int role = 0; role < roles.Length; role++)
                {
                    if (players[ply].Role == roles[role])
                    {
                        result++;
                        break;
                    }
                }
            }

            return result;
        }

        public static List<T> GetRole<T>(this List<MMPlayer> players)
            where T : MMCustomRole
        {
            List<T> result = new List<T>();

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i]?.CustomRole is T role)
                    result.Add(role);
            }

            return result;
        }
    }
}
