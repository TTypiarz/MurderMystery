using Exiled.API.Extensions;
using MEC;
using MurderMystery.API.Enums;
using MurderMystery.API.Roles;
using MurderMystery.Extensions;
using System;
using System.Collections.Generic;

namespace MurderMystery.API.Features
{
    public abstract class CustomRole
    {
        internal static readonly Dictionary<MMRole, CustomRole> Roles = new Dictionary<MMRole, CustomRole>()
        {
            [MMRole.None] = null,
            [MMRole.Spectator] = new Spectator(),
            [MMRole.Innocent] = new Innocent(),
            [MMRole.Murderer] = new Murderer(),
            [MMRole.Detective] = new Detective()
        };

        public static bool GetCustomRole(MMRole role, out CustomRole customRole)
        {
            return Roles.TryGetValue(role, out customRole);
        }

        public abstract string Name { get; }
        public abstract string ColoredName { get; }
        public abstract Misc.PlayerInfoColorTypes PlayerInfoColor { get; }

        public abstract MMRole Role { get; }
        public abstract string SpawnMsg { get; }
        public abstract string SpawnInfoMsg { get; }

        public virtual MMRole[] RolesCanView { get; } = new MMRole[] { MMRole.Spectator };

        internal virtual void OnFirstSpawn(MMPlayer player)
        {
            player.RemoveInvalidItems();

            if (player.Role != MMRole.Spectator)
            {
                try
                {
                    CustomItem.SerialItems.Add(player.Player.AddItem(ItemType.ArmorCombat).Serial, CustomItem.Items[MMItem.UnprotectedItem]);
                }
                catch { }
            }

            player.Player.ReferenceHub.gameConsoleTransmission.SendToClient(player.Player.Connection, $"You have spawned as:\n<size=70>{ColoredName}</size>", "white");

            try
            {
                player.Player.ShowHint($"\n\n\n\n\n\n{SpawnMsg}\n{SpawnInfoMsg}", 20);
            }
            catch { } // Catch nullref in exiled postfix patch.

            foreach (MMPlayer ply in MMPlayer.List.GetRoles(RolesCanView))
            {
                ply.Player.SetPlayerInfoForTargetOnly(player.Player, this.CustomInfoColoredName());
            }
        }

        internal void InternalChangingMMRole(MMPlayer player, CustomRole newRole)
        {
            try
            {
                ChangingMMRole(player, newRole);

                foreach (MMPlayer ply in this.PlayersCanView())
                {
                    ply.Player.SetPlayerInfoForTargetOnly(player.Player, string.Empty);
                }

                foreach (MMPlayer ply in this.CanSeePlayers())
                {
                    player.Player.SetPlayerInfoForTargetOnly(ply.Player, string.Empty);
                }

                player.RemoveEquipmentItems();
                player.RemoveInvalidItems();
            }
            catch (Exception e)
            {
                MMLog.Error(e, $"An error occured in the role: {Name}");
            }
        }
        internal void InternalChangedMMRole(MMPlayer player, CustomRole oldRole)
        {
            try
            {
                ChangedMMRole(player, oldRole);

                foreach (MMPlayer ply in this.PlayersCanView())
                {
                    ply.Player.SetPlayerInfoForTargetOnly(player.Player, this.CustomInfoColoredName());
                }

                foreach (MMPlayer ply in this.CanSeePlayers())
                {
                    player.Player.SetPlayerInfoForTargetOnly(ply.Player, ply.CustomRole.CustomInfoColoredName());
                }

                player.Player.Broadcast(5, $"<size=30>Your role has been changed to {ColoredName}</size>");
            }
            catch (Exception e)
            {
                MMLog.Error(e, $"An error occured in the role: {Name}");
            }
        }
        protected virtual void ChangingMMRole(MMPlayer player, CustomRole newRole) { }
        protected virtual void ChangedMMRole(MMPlayer player, CustomRole oldRole) { }
    }
}
