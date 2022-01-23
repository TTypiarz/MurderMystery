using CommandSystem;
using Exiled.Permissions.Extensions;
using InventorySystem.Items;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MurderMystery.Extensions
{
    public static class PlayerExtensions
    {
        public static bool CheckPermission(this ICommandSender sender, MMPerm perm)
        {
            if (perm != MMPerm.None)
            {
                return sender.CheckPermission(perm.ToPermissionString());
            }
            else
            {
                return true;
            }
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

        public static void RemoveEquipmentItems(this MMPlayer player)
        {
            foreach (ushort key in CustomItem.SerialItems.Keys.Where(x => CustomItem.SerialItems[x].IsEquipmentItem))
            {
                if (player.Player.Inventory.UserInventory.Items.TryGetValue(key, out ItemBase itemBase))
                {
                    if (player.Player.Inventory.NetworkCurItem.SerialNumber == key)
                    {
                        player.Player.Inventory.NetworkCurItem = ItemIdentifier.None;
                        player.Player.Inventory.CurInstance = null;
                    }

                    player.Player.Inventory.UserInventory.Items.Remove(key);
                    UnityEngine.Object.Destroy(itemBase);
                }
            }
            player.Player.Inventory.SendItemsNextFrame = true;
        }

        public static void RemoveInvalidItems(this MMPlayer player)
        {
            List<ushort> serials = player.Player.Inventory.UserInventory.Items.Keys.ToList();

            foreach (ushort key in serials)
            {
                if (!CustomItem.SerialItems.TryGetValue(key, out _))
                {
                    if (player.Player.Inventory.NetworkCurItem.SerialNumber == key)
                    {
                        player.Player.Inventory.NetworkCurItem = ItemIdentifier.None;
                        player.Player.Inventory.CurInstance = null;
                    }

                    player.Player.Inventory.UserInventory.Items.Remove(key);
                }
            }
            player.Player.Inventory.SendItemsNextFrame = true;
        }
    }
}
