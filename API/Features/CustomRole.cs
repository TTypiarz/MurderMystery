using MEC;
using MurderMystery.API.Enums;
using MurderMystery.API.Roles;
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

        public abstract MMRole Role { get; }
        public abstract string SpawnMsg { get; }
        public abstract string SpawnInfoMsg { get; }

        internal virtual void OnFirstSpawn(MMPlayer player)
        {
            if (player.Role != MMRole.Spectator)
            {
                try
                {
                    player.Player.AddItem(ItemType.ArmorCombat);
                }
                catch { }
            }

            player.Player.ReferenceHub.gameConsoleTransmission.SendToClient(player.Player.Connection, $"You have spawned as:\n<size=70>{ColoredName}</size>", "white");

            try
            {
                player.Player.ShowHint($"\n\n\n\n\n\n{SpawnMsg}\n{SpawnInfoMsg}", 20);
            }
            catch { } // Catch nullref in exiled postfix patch.
        }
        internal virtual void ChangingMMRole(MMPlayer player, CustomRole newRole) { }
        internal virtual void ChangedMMRole(MMPlayer player, CustomRole oldRole) { }
    }
}
