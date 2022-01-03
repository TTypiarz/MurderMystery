using MurderMystery.API.Enums;
using MurderMystery.API.Roles;
using System.Collections.Generic;

namespace MurderMystery.API.Features
{
    public abstract class CustomRole
    {
        private static readonly Dictionary<MMRole, CustomRole> Roles = new Dictionary<MMRole, CustomRole>()
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

        public abstract MMRole Role { get; }
        public abstract string SpawnMsg { get; }
        public abstract string SpawnInfoMsg { get; }

        internal virtual void OnFirstSpawn(MMPlayer player)
        {
            player.Player.SendConsoleMessage($"You spawned as: {ColoredName} this round.", "white");

            player.Player.ShowHint($"\n\n\n\n\n\n{SpawnMsg}\n{SpawnInfoMsg}", 20);

            player.Player.Ammo[ItemType.Ammo12gauge] = 0;
            player.Player.Ammo[ItemType.Ammo44cal] = 0;
            player.Player.Ammo[ItemType.Ammo556x45] = 0;
            player.Player.Ammo[ItemType.Ammo762x39] = 0;
            player.Player.Ammo[ItemType.Ammo9x19] = 0;
            player.Player.Inventory.SendAmmoNextFrame = true;

            player.Player.ClearInventory();
        }
        internal virtual void ChangingMMRole(MMPlayer player, CustomRole newRole) { }
        internal virtual void ChangedMMRole(MMPlayer player, CustomRole oldRole) { }
    }
}
