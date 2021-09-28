﻿using Exiled.API.Features;
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
                        role.ChangedMMRole(this, null);
                    }
                    else
                    {
                        if (role == null)
                        {
                            CustomRole.ChangingMMRole(this, null);
                            CustomRole = null;
                        }
                        else
                        {
                            CustomRole oldRole = CustomRole;
                            CustomRole.ChangingMMRole(this, role);
                            CustomRole = role;
                            role.ChangedMMRole(this, oldRole);
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

        internal void Verified()
        {
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