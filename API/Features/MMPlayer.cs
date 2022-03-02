using Exiled.API.Features;
using MEC;
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

        /// <summary>
        /// Used for detective kill count.
        /// </summary>
        public int InnocentKills { get; internal set; } = 0;

        /// <summary>
        /// Used to tell whether an innocent is free kill or not.
        /// </summary>
        public bool FreeKill { get; set; } = false;

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
                Player.Broadcast(15, "<size=30>Murder Mystery gamemode is currently active.\n<b>[Check console for details.]</b></size>");
                Timing.CallDelayed(1f, () =>
                    Player.ReferenceHub.gameConsoleTransmission.SendToClient(Player.Connection, string.Join("\n", MurderMystery.GamemodeInformation), "white"));

                SetRoleSilently(MMRole.Spectator);
            }
            else
            {
                Player.Broadcast(15, "<size=30>Murder Mystery gamemode is enabled for this round.\n<b>[Check console for details.]</b></size>");
                Timing.CallDelayed(1f, () =>
                    Player.ReferenceHub.gameConsoleTransmission.SendToClient(Player.Connection, string.Join("\n", MurderMystery.GamemodeInformation), "white"));
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
