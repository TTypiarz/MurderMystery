using Exiled.API.Features;
using MurderMystery.API.Enums;
using System;
using System.Collections.Generic;

namespace MurderMystery.API.Features
{
    public class MMPlayer
    {
        internal MMPlayer(Player player) => Player = player;

        public static List<MMPlayer> List { get; } = new List<MMPlayer>();

        public Player Player { get; }
        public MMRole Role
        {
            get
            {
                return CustomRole == null ? MMRole.None : CustomRole.Role;
            }
            set
            {
                if (CustomRole.GetCustomRole(value, out CustomRole role))
                {
                    if (value == Role)
                    {
                        // Current role value and given role value are the same, so do nothing.
                        return;
                    }

                    if (CustomRole == null)
                    {
                        if (role == null)
                            return;

                        CustomRole = role;
                        role.InternalChangedMMRole(this, null);
                    }
                    else
                    {
                        if (role == null)
                        {
                            CustomRole.InternalChangingMMRole(this, null);
                            CustomRole = null;
                        }
                        else
                        {
                            CustomRole oldRole = CustomRole;
                            CustomRole.InternalChangingMMRole(this, role);
                            CustomRole = role;
                            role.InternalChangedMMRole(this, oldRole);
                        }
                    }
                }
                else
                {
                    // Given value is invalid.
                    throw new ArgumentException(nameof(value));
                }
            }
        }
        public CustomRole CustomRole { get; private set; }

        public static bool Get(Player ply, out MMPlayer player)
        {
            if (ply == null)
            {
                player = null;
                return false;
            }

            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].Player == ply)
                {
                    player = List[i];
                    return true;
                }
            }

            player = null;
            return false;
        }

        public static bool Get(ReferenceHub ply, out MMPlayer player)
        {
            return Get(Player.Get(ply), out player);
        }

        internal void Verified()
        {
            if (MurderMystery.Singleton.GamemodeManager.Started)
            {
                Player.Broadcast(15, "<size=30>Murder Mystery gamemode is currently active.</size>");

                Role = MMRole.Spectator;
            }
            else
            {
                Player.Broadcast(15, "<size=30>Murder Mystery gamemode is enabled for this round.</size>");
            }
        }
        internal void Destroying()
        {
        }

        internal void SetRoleSilently(MMRole value)
        {
            if (CustomRole.GetCustomRole(value, out CustomRole customRole))
            {
                CustomRole = customRole;
            }
            else
            {
                throw new ArgumentException(nameof(value));
            }
        }
    }
}
