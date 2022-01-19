using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Pickups;
using MapGeneration.Distributors;
using MEC;
using MurderMystery.API;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;
using MurderMystery.API.Interfaces;
using MurderMystery.Extensions;
using MurderMystery.Patches;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using Handlers = Exiled.Events.Handlers;

namespace MurderMystery
{
    public class GamemodeManager
    {
        internal GamemodeManager() { }

        public bool PrimaryEnabled { get; private set; } = false;
        public bool PlayerEnabled { get; private set; } = false;
        public bool GamemodeEnabled { get; private set; } = false;

        public bool WaitingPlayers { get; private set; } = false;
        public bool Started { get; private set; } = false;

        public bool FailedToStart { get; private set; } = false;
        public float TimeUntilEnd { get; set; } = 0f;

        public bool Murderers939Vision { get; private set; } = false;

        public Random Rng { get; private set; } = new Random();
        public CoroutineHandle[] GamemodeCoroutines { get; } = new CoroutineHandle[2]
        {
            default, // [0] SpectatorText
            default  // [1] RoundTimer
        };

        internal void ToggleGamemode(bool enable)
        {
            try
            {
                if (enable ^ PrimaryEnabled)
                {
                    MMLog.Debug($"{(enable ? "Enabling" : "Disabling")} the murder mystery gamemode.");

                    if (enable)
                    {
                        ToggleEvent(MMEventType.Primary, true);
                    }
                    else
                    {
                        ToggleEvent(MMEventType.Primary, false);
                        ToggleEvent(MMEventType.Player, false);
                        ToggleEvent(MMEventType.Gamemode, false);

                        RespawnTimerPatch.TogglePatch(false);

                        Timing.KillCoroutines(GamemodeCoroutines);

                        Started = false;
                        WaitingPlayers = false;
                        FailedToStart = false;
                        TimeUntilEnd = -1f;
                        Murderers939Vision = false;

                        ServerConsole.FriendlyFire = GameCore.ConfigFile.ServerConfig.GetBool("friendly_fire");
                        FriendlyFireConfig.PauseDetector = false;

                        CustomItemPool.ProtectedItemIds.Clear();
                    }
                }
                else
                {
                    MMLog.Debug($"\nCall invalid: {(enable ? "Enabling" : "Disabling")}\nCaller: {MMUtilities.GetCallerString()}");
                }
            }
            catch (Exception e)
            {
                MMLog.Error($"FATAL ERROR:\n{e}");
            }
        }
        internal void ToggleEvent(MMEventType eventType, bool enable)
        {
            try
            {
                MMLog.Debug($"{(enable ? "Enabling" : "Disabling")} {eventType} events.");

                switch (eventType)
                {
                    case MMEventType.Primary:
                        if (enable ^ PrimaryEnabled)
                        {
                            if (enable)
                            {
                                Handlers.Server.WaitingForPlayers += WaitingForPlayers;
                                Handlers.Server.RoundStarted += RoundStarted;
                                Handlers.Server.RoundEnded += RoundEnded;
                                Handlers.Server.RestartingRound += RestartingRound;
                                Handlers.Map.SpawningItem += SpawningItem;

                                MurderMystery.Singleton.Harmony.Patch(LateRoundStartPatch.Original, null, LateRoundStartPatch.Patch);
                                MurderMystery.Singleton.Harmony.Patch(RoundSummaryPatch.Original, RoundSummaryPatch.Patch);
                            }
                            else
                            {
                                Handlers.Server.WaitingForPlayers -= WaitingForPlayers;
                                Handlers.Server.RoundStarted -= RoundStarted;
                                Handlers.Server.RoundEnded -= RoundEnded;
                                Handlers.Server.RestartingRound -= RestartingRound;
                                Handlers.Map.SpawningItem -= SpawningItem;

                                MurderMystery.Singleton.Harmony.Unpatch(LateRoundStartPatch.Original, HarmonyLib.HarmonyPatchType.Postfix);
                                MurderMystery.Singleton.Harmony.Unpatch(RoundSummaryPatch.Original, HarmonyLib.HarmonyPatchType.Prefix);
                            }

                            PrimaryEnabled = enable;
                        }
                        return;

                    case MMEventType.Player:
                        if (enable ^ PlayerEnabled)
                        {
                            if (enable)
                            {
                                Handlers.Player.Verified += Verified;
                                Handlers.Player.Destroying += Destroying;
                            }
                            else
                            {
                                Handlers.Player.Verified -= Verified;
                                Handlers.Player.Destroying -= Destroying;
                            }

                            PlayerEnabled = enable;
                        }
                        return;


                    case MMEventType.Gamemode:
                        if (enable ^ GamemodeEnabled)
                        {
                            if (enable)
                            {
                                Handlers.Player.ChangingRole += ChangingRole;
                                Handlers.Player.Spawning += Spawning;
                                Handlers.Player.Dying += Dying;
                                Handlers.Player.DroppingItem += DroppingItem;
                                Handlers.Player.Shooting += Shooting;
                                Handlers.Player.PickingUpItem += PickingUpItem;
                                Handlers.Player.DroppingAmmo += DroppingAmmo;
                                Handlers.Player.ReloadingWeapon += ReloadingWeapon;
                                Handlers.Player.SpawningRagdoll += SpawningRagdoll;
                                Handlers.Server.RespawningTeam += RespawningTeam;
                                Handlers.Server.EndingRound += EndingRound;

                                ServerConsole.FriendlyFire = true;
                                FriendlyFireConfig.PauseDetector = true;
                                DependencyUtilities.HandleCedModV3(true);
                                RespawnTimerPatch.TogglePatch(true);
                            }
                            else
                            {
                                Handlers.Player.ChangingRole -= ChangingRole;
                                Handlers.Player.Spawning -= Spawning;
                                Handlers.Player.Dying -= Dying;
                                Handlers.Player.DroppingItem -= DroppingItem;
                                Handlers.Player.Shooting -= Shooting;
                                Handlers.Player.PickingUpItem -= PickingUpItem;
                                Handlers.Player.DroppingAmmo -= DroppingAmmo;
                                Handlers.Player.ReloadingWeapon -= ReloadingWeapon;
                                Handlers.Player.SpawningRagdoll -= SpawningRagdoll;
                                Handlers.Server.RespawningTeam -= RespawningTeam;
                                Handlers.Server.EndingRound -= EndingRound;

                                DependencyUtilities.HandleCedModV3(false);
                                RespawnTimerPatch.TogglePatch(false);
                            }

                            GamemodeEnabled = enable;
                        }
                        return;
                }
            }
            catch (Exception e)
            {
                MMLog.Error($"FATAL ERROR:\n{e}");
            }
        }

        #region Primary Events

        private void WaitingForPlayers()
        {
            if (!PlayerEnabled)
            {
                MMLog.Debug("Primary event called. Enabling player events...");

                ToggleEvent(MMEventType.Player, true);

                WaitingPlayers = true;
            }
        }

        private void RoundStarted()
        {
            if (WaitingPlayers)
            {
                MMLog.Debug("Primary event called. Enabling gamemode events and Starting gamemode...");

                ToggleEvent(MMEventType.Gamemode, true);

                StartGamemode();

                Started = true;
            }
        }

        private void RoundEnded(RoundEndedEventArgs ev)
        {
            if (GamemodeEnabled)
            {
                MMLog.Debug("Primary event called. Disabling gamemode events...");

                ToggleEvent(MMEventType.Gamemode, false);
            }
        }

        private void RestartingRound()
        {
            if (Started)
            {
                MMLog.Debug("Primary event called. Disabling gamemode...");

                ToggleGamemode(false);
            }
        }

        private void SpawningItem(SpawningItemEventArgs ev)
        {
            if (!MurderMystery.AllowedItems.Contains(ev.Pickup.Type))
                ev.IsAllowed = false;
        }

        #endregion

        #region Player Events

        private void Verified(VerifiedEventArgs ev)
        {
            MMPlayer player = new MMPlayer(ev.Player);

            player.Verified();

            MMPlayer.List.Add(player);

            MMLog.Debug("Player verified. (Added to list)");
        }

        private void Destroying(DestroyingEventArgs ev)
        {
            if (MMPlayer.Get(ev.Player, out MMPlayer player))
            {
                player.Destroying();

                MMPlayer.List.Remove(player);

                MMLog.Debug("Player destroying. (Removed from list)");
            }
        }

        #endregion

        #region Gamemode Events

        private void ChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Reason == Exiled.API.Enums.SpawnReason.RoundStart)
            {
                ev.NewRole = RoleType.ClassD;

                //ev.Ammo[ItemType.Ammo9x19] = 30;
                //ev.Ammo[ItemType.Ammo44cal] = 18;
                ev.Items.Clear();
            }
            else if (ev.Reason == Exiled.API.Enums.SpawnReason.LateJoin)
            {
                if (MMPlayer.Get(ev.Player, out MMPlayer player))
                {
                    ev.NewRole = RoleType.ClassD;
                    player.SetRoleSilently(MMRole.Innocent);
                    Timing.CallDelayed(1f, () =>
                    {
                        player.CustomRole?.OnFirstSpawn(player);
                    });

                    //ev.Ammo[ItemType.Ammo9x19] = 30;
                    //ev.Ammo[ItemType.Ammo44cal] = 18;
                    ev.Items.Clear();
                }
                else
                {
                    ev.NewRole = RoleType.Spectator;
                }
            }
            /*else
            {
                if (MMPlayer.Get(ev.Player, out MMPlayer player))
                {
                    player.Role = MMRole.Spectator;
                }
            }*/
        }

        private void Spawning(SpawningEventArgs ev)
        {
            ev.Position = RoleType.Scp049.GetRandomSpawnProperties().Item1;
        }

        private void RespawningTeam(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
            ev.MaximumRespawnAmount = 0;
            ev.Players.Clear();
        }

        private void EndingRound(EndingRoundEventArgs ev)
        {
            ev.IsAllowed = false;
            ev.IsRoundEnded = false;

            if (Round.ElapsedTime.TotalSeconds < 5)
                return;

            int innocents = MMPlayer.List.GetRolesCount(MMRole.Innocent, MMRole.Detective);
            int murderers = MMPlayer.List.GetRoleCount(MMRole.Murderer);

            if (innocents == 0 && murderers > 0)
            {
                Map.Broadcast(300, "\n<size=80><color=#ff0000><b>Murderers win</b></color></size>\n<size=30>All innocents have been killed.</size>", Broadcast.BroadcastFlags.Normal, true);
                goto Allow;
            }

            if (innocents > 0 && murderers == 0)
            {
                Map.Broadcast(300, "\n<size=80><color=#00ff00><b>Innocents win</b></color></size>\n<size=30>All murderers have been killed.</size>", Broadcast.BroadcastFlags.Normal, true);
                goto Allow;
            }

            if (innocents == 0 && murderers == 0)
            {
                Map.Broadcast(300, "\n<size=80><color=#7f7f7f><b>Stalemate</b></color></size>\n<size=30>All players have been killed. HOW??? ( ͡° ͜ʖ ͡°)</size>", Broadcast.BroadcastFlags.Normal, true);
                goto Allow;
            }

            if (GamemodeCoroutines[1].IsRunning && TimeUntilEnd <= 0f)
            {
                Map.Broadcast(300, "\n<size=80><color=#00ff00><b>Innocents win</b></color></size>\n<size=30>Murderers have run out of time, and lost.</size>", Broadcast.BroadcastFlags.Normal, true);
                goto Allow;
            }

            return;

        Allow:
            ev.IsAllowed = true;
            ev.IsRoundEnded = true;
        }

        private void Dying(DyingEventArgs ev)
        {
            if (MMPlayer.Get(ev.Target, out MMPlayer ply))
            {
                if (MMPlayer.Get(ev.Killer, out MMPlayer killer))
                {
                    if (ply.Role == MMRole.Innocent && killer.Role == MMRole.Detective)
                    {
                        killer.Player.ReferenceHub.playerStats.DealDamage(new CustomReasonDamageHandler("Shot an innocent player.", 1000000, "L O L"));
                    }
                }

                ply.Role = MMRole.Spectator;
            }

            ev.Target.CustomInfo = string.Empty;
            ev.Target.Ammo.Clear();
        }

        private void DroppingItem(DroppingItemEventArgs ev)
        {
            if (CustomItemPool.ProtectedItemIds.Contains(ev.Item.Serial))
                ev.IsAllowed = false;
        }

        private void DroppingAmmo(DroppingAmmoEventArgs ev)
        {
            ev.IsAllowed = false;
            ev.Amount = 0;
        }

        private void PickingUpItem(PickingUpItemEventArgs ev)
        {
            if (MMPlayer.Get(ev.Player, out MMPlayer player))
            {
                if (CustomItemPool.ProtectedItemIds.Contains(ev.Pickup.Serial))
                {
                    switch (ev.Pickup.Type)
                    {
                        case ItemType.GunRevolver:
                            if (player.Role == MMRole.Innocent)
                            {
                                ev.IsAllowed = true;
                                player.Role = MMRole.Detective;
                                return;
                            }
                            else
                            {
                                ev.IsAllowed = false;
                                return;
                            }

                        default:
                            ev.IsAllowed = false;
                            return;
                    }
                }
            }
        }

        private void Shooting(ShootingEventArgs ev)
        {
            if (MMPlayer.Get(Player.Get(ev.TargetNetId), out MMPlayer target) && MMPlayer.Get(ev.Shooter, out MMPlayer shooter))
            {
                if (target.Role == shooter.Role)
                {
                    shooter.Player.ShowHint("You cannot shoot your teammates.", 3);
                    ev.IsAllowed = false;
                }
            }
        }

        private void ReloadingWeapon(ReloadingWeaponEventArgs ev)
        {
            ev.Player.SetAmmo(ev.Firearm.AmmoType, ev.Firearm.MaxAmmo);
        }

        private void SpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            if (MMPlayer.Get(ev.Owner, out MMPlayer player))
            {
                if (player.Role == MMRole.None || player.Role == MMRole.Spectator)
                {
                    ev.IsAllowed = false;
                    return;
                }

                ev.Nickname += $" [{player.CustomRole.ColoredName}]";
            }
            else
            {
                ev.IsAllowed = false;
            }
        }

        #endregion

        #region Primary Functions
        private void StartGamemode()
        {
            const DoorLockReason fullLock =
                DoorLockReason.AdminCommand
                | DoorLockReason.DecontLockdown
                | DoorLockReason.SpecialDoorFeature
                | DoorLockReason.NoPower
                | DoorLockReason.Lockdown2176;

            try
            {
                MMLog.Debug("Primary function called.");

                // Make workstations un-usable to prevent attachment changes by players.
                foreach (WorkstationController controller in WorkstationController.AllWorkstations)
                {
                    controller.NetworkStatus = 4;
                }

                foreach (Door door in Map.Doors)
                {
                    if (door.Base is CheckpointDoor chkDoor)
                    {
                        foreach (DoorVariant subDoor in chkDoor._subDoors)
                        {
                            subDoor.NetworkTargetState = false;
                            subDoor.NetworkActiveLocks = (ushort)fullLock;
                            subDoor.UsedBy106 = false;
                        }

                        chkDoor.NetworkTargetState = false;
                        chkDoor.NetworkActiveLocks = (ushort)fullLock;
                        chkDoor.UsedBy106 = false;
                    }
                    else if (door.RequiredPermissions.RequiredPermissions > 0)
                    {
                        door.Base.NetworkTargetState = true;
                        door.Base.NetworkActiveLocks = (ushort)fullLock;
                    }
                }

                foreach (Lift lift in Map.Lifts)
                {
                    switch (lift.Type())
                    {
                        case Exiled.API.Enums.ElevatorType.LczA:
                        case Exiled.API.Enums.ElevatorType.LczB:
                            lift.Network_locked = true;
                            continue;
                    }
                }

                foreach (Pickup item in Map.Pickups)
                {
                    if (!MurderMystery.AllowedItems.Contains(item.Type))
                    {
                        item.Destroy();

                    }
                }


                try
                {
                    UnityEngine.Object.FindObjectOfType<Recontainer079>()._alreadyRecontained = true;
                }
                catch { }

                Timing.CallDelayed(MurderMystery.Singleton.Config.EquipmentDelay, () =>
                {
                    foreach (CustomRole role in CustomRole.Roles.Values)
                    {
                        if (role is IEquipment equipment)
                        {
                            List<MMPlayer> players = MMPlayer.List.GetRole(role.Role);

                            for (int i = 0; i < players.Count; i++)
                            {
                                equipment.GiveEquipment(players[i]);

                                players[i].Player.Broadcast(10, equipment.EquipmentMessage);
                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                MMLog.Error($"FATAL ERROR:\n{e}");
                Map.Broadcast(15, "<size=30>Murder Mystery gamemode failed to start. Round restart in 10 seconds.</size>", Broadcast.BroadcastFlags.Normal, true);
                Timing.CallDelayed(10, () =>
                {
                    MurderMystery.Singleton.GamemodeManager.ToggleGamemode(false);
                    Round.Restart(false);
                });
                FailedToStart = true;
            }
        }

        internal void LateStartGamemode()
        {
            try
            {
                MMLog.Debug("Primary function called.");

                if (FailedToStart)
                    return;

                for (int i = 0; i < MMPlayer.List.Count; i++)
                {
                    if (MMPlayer.List[i].Player.IsOverwatchEnabled || MMPlayer.List[i].Player.Role == RoleType.Spectator)
                        MMPlayer.List[i].SetRoleSilently(MMRole.Spectator);
                }

                List<MMPlayer> players = new List<MMPlayer>(MMPlayer.List.GetRole(MMRole.None));

                if (!MurderMystery.Singleton.Config.CalculateRoles(players.Count, out int m, out int d))
                {
                    throw new Exception("Invalid role calculation.");
                }

                int totalm = 0;
                while (totalm < m)
                {
                    int index = Rng.Next(players.Count);
                    players[index].SetRoleSilently(MMRole.Murderer);
                    players.RemoveAt(index);
                    totalm++;
                }
                int totald = 0;
                while (totald < d)
                {
                    int index = Rng.Next(players.Count);
                    players[index].SetRoleSilently(MMRole.Detective);
                    players.RemoveAt(index);
                    totald++;
                }

                for (int i = 0; i < players.Count; i++)
                {
                    players[i].SetRoleSilently(MMRole.Innocent);
                }

                for (int i = 0; i < MMPlayer.List.Count; i++)
                {
                    MMPlayer.List[i]?.CustomRole?.OnFirstSpawn(MMPlayer.List[i]);
                }

                GamemodeCoroutines[0] = Timing.RunCoroutine(SpectatorText());

                if (MurderMystery.Singleton.Config.RoundTime != 0)
                {
                    TimeUntilEnd = MurderMystery.Singleton.Config.RoundTime;
                    GamemodeCoroutines[1] = Timing.RunCoroutine(RoundTimer());
                }
            }
            catch (Exception e)
            {
                MMLog.Error($"FATAL ERROR:\n{e}");
                Map.ClearBroadcasts();
                Map.Broadcast(15, "<size=30>Murder Mystery gamemode failed to start. Round restart in 10 seconds.</size>");
                Timing.CallDelayed(10, () =>
                {
                    MurderMystery.Singleton.GamemodeManager.ToggleGamemode(false);
                    Round.Restart(false);
                });
                FailedToStart = true;
            }
        }
        
        private IEnumerator<float> SpectatorText()
        {
            MMLog.Debug("[GamemodeManager::SpectatorText]", "Primary function called.");

            while (GamemodeEnabled)
            {
                yield return Timing.WaitForSeconds(1f);

                for (int i = 0; i < MMPlayer.List.Count; i++)
                {
                    MMPlayer ply = MMPlayer.List[i];

                    if (ply.Player.Role == RoleType.Spectator)
                    {
                        if (MMPlayer.Get(ply.Player.ReferenceHub.spectatorManager.CurrentSpectatedPlayer, out MMPlayer spectated) && ply != spectated)
                        {
                            ply.Player.ShowHint($"\n\n\n\n\n\n\n\n<size=40>You are spectating: {spectated.Player.Nickname}\n" +
                                $"{(spectated.CustomRole == null ? "<b>They have no role.</b>" : $"<b>They are: {spectated.CustomRole.ColoredName}")}</size></b>", 2);
                        }
                    }
                }
            }
        }

        private IEnumerator<float> RoundTimer()
        {
            Config cfg = MurderMystery.Singleton.Config;

            while (GamemodeEnabled)
            {
                yield return Timing.WaitForOneFrame;
                TimeUntilEnd -= Timing.DeltaTime;

                if (!Murderers939Vision && TimeUntilEnd <= cfg.Murderers939VisionTime && cfg.Murderers939VisionTime != 0)
                {
                    List<MMPlayer> players = MMPlayer.List.GetRole(MMRole.Murderer);

                    for (int i = 0; i < players.Count; i++)
                    {
                        players[i].Player.EnableEffect<CustomPlayerEffects.Visuals939>();
                        players[i].Player.Broadcast(5, "<size=30>You can now see players through walls.</size>");
                    }

                    Murderers939Vision = true;
                }
            }
        }
        #endregion
    }
}
