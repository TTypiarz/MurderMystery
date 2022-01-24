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

        public bool RestartedRound { get; private set; } = false;
        public bool WaitingPlayers { get; private set; } = false;
        public bool Started { get; private set; } = false;

        public bool FailedToStart { get; private set; } = false;
        public float TimeUntilEnd { get; set; } = 0f;
        public int GeneratorsActivated { get; private set; } = 0;

        public bool Murderers939Vision { get; private set; } = false;
        public bool GeneratorsUnlocked { get; private set; } = false;

        public Random Rng { get; private set; } = new Random();
        public CoroutineHandle[] GamemodeCoroutines { get; } = new CoroutineHandle[]
        {
            default, // [0] PlayerText
            default, // [1] RoundTimer
            default  // [2] EquipmentTimer
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

                        Timing.KillCoroutines(GamemodeCoroutines);

                        Started = false;
                        WaitingPlayers = false;
                        FailedToStart = false;
                        TimeUntilEnd = -1f;
                        Murderers939Vision = false;
                        GeneratorsUnlocked = false;
                        RestartedRound = false;
                        GeneratorsActivated = 0;

                        ServerConsole.FriendlyFire = GameCore.ConfigFile.ServerConfig.GetBool("friendly_fire");
                        FriendlyFireConfig.PauseDetector = false;

                        CustomItem.SerialItems.Clear();
                        MMPlayer.List.Clear();
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
                            }
                            else
                            {
                                Handlers.Server.WaitingForPlayers -= WaitingForPlayers;
                                Handlers.Server.RoundStarted -= RoundStarted;
                                Handlers.Server.RoundEnded -= RoundEnded;
                                Handlers.Server.RestartingRound -= RestartingRound;
                                Handlers.Map.SpawningItem -= SpawningItem;
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

                                DependencyUtilities.HandleCommonUtils(true);
                            }
                            else
                            {
                                Handlers.Player.Verified -= Verified;
                                Handlers.Player.Destroying -= Destroying;

                                DependencyUtilities.HandleCommonUtils(false);
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
                                Handlers.Player.Died += Died;
                                Handlers.Player.DroppingItem += DroppingItem;
                                Handlers.Player.Shooting += Shooting;
                                Handlers.Player.PickingUpItem += PickingUpItem;
                                Handlers.Player.Hurting += Hurting;
                                Handlers.Player.DroppingAmmo += DroppingAmmo;
                                Handlers.Player.ReloadingWeapon += ReloadingWeapon;
                                Handlers.Player.SpawningRagdoll += SpawningRagdoll;
                                Handlers.Server.RespawningTeam += RespawningTeam;
                                Handlers.Server.EndingRound += EndingRound;
                                Handlers.Map.GeneratorActivated += GeneratorActivated;

                                ServerConsole.FriendlyFire = true;
                                FriendlyFireConfig.PauseDetector = true;
                                DependencyUtilities.HandleCedModV3(true);
                            }
                            else
                            {
                                Handlers.Player.ChangingRole -= ChangingRole;
                                Handlers.Player.Spawning -= Spawning;
                                Handlers.Player.Dying -= Dying;
                                Handlers.Player.Died -= Died;
                                Handlers.Player.DroppingItem -= DroppingItem;
                                Handlers.Player.Shooting -= Shooting;
                                Handlers.Player.PickingUpItem -= PickingUpItem;
                                Handlers.Player.Hurting -= Hurting;
                                Handlers.Player.DroppingAmmo -= DroppingAmmo;
                                Handlers.Player.ReloadingWeapon -= ReloadingWeapon;
                                Handlers.Player.SpawningRagdoll -= SpawningRagdoll;
                                Handlers.Server.RespawningTeam -= RespawningTeam;
                                Handlers.Server.EndingRound -= EndingRound;
                                Handlers.Map.GeneratorActivated -= GeneratorActivated;

                                DependencyUtilities.HandleCedModV3(false);
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
                RestartedRound = true;
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
            else
            {
                RestartedRound = true;
            }
        }

        private void SpawningItem(SpawningItemEventArgs ev)
        {
            if (RestartedRound)
            {
                if (!MurderMystery.AllowedItems.Contains(ev.Pickup.Type))
                {
                    ev.IsAllowed = false;
                    return;
                }

                try
                {
                    CustomItem.SerialItems.Add(ev.Pickup.Serial, CustomItem.Items[MMItem.UnprotectedItem]);
                }
                catch { }
            }
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
                ev.Items.Clear();
                ev.Ammo.Clear();
            }
            else if (ev.Reason == Exiled.API.Enums.SpawnReason.LateJoin && ev.NewRole != RoleType.Spectator)
            {
                if (MMPlayer.Get(ev.Player, out MMPlayer player))
                {
                    ev.NewRole = RoleType.ClassD;
                    player.SetRoleSilently(MMRole.Innocent);
                    Timing.CallDelayed(1f, () =>
                    {
                        player.CustomRole?.OnFirstSpawn(player);
                    });

                    ev.Items.Clear();
                    ev.Ammo.Clear();
                }
                else
                {
                    ev.NewRole = RoleType.Spectator;
                }
            }
            else if (ev.Reason == Exiled.API.Enums.SpawnReason.LateJoin && ev.NewRole == RoleType.Spectator)
            {
                if (MMPlayer.Get(ev.Player, out MMPlayer player))
                {
                    player.Role = MMRole.Spectator;
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

            if (!MurderMystery.InternalDebugSingleplayer)
            {
                if (GeneratorsActivated == 3)
                {
                    Map.Broadcast(300, "\n<size=80><color=#ff0000><b>Murderers win</b></color></size>\n<size=30>All generators have been activated.</size>", Broadcast.BroadcastFlags.Normal, true);
                    goto Allow;
                }

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
                        if (killer.InnocentKills++ >= 2)
                        {
                            CustomReasonDamageHandler customReason = new CustomReasonDamageHandler("Shot too many innocent players.")
                            {
                                Damage = 10000000
                            }; // legit a base-game nullref in the other method, northwood moment

                            ev.Killer.ReferenceHub.playerStats.DealDamage(customReason);
                        }
                        else
                        {
                            try
                            {
                                killer.Player.ShowHint("<size=30>You have killed an innocent player.\n<b>Do not kill any more innocents or you will be slain.</b></size>", 7);
                            }
                            catch { }
                        }
                    }
                }
            }

            ev.Target.Ammo.Clear();
        }

        private void Died(DiedEventArgs ev)
        {
            if (MMPlayer.Get(ev.Target, out MMPlayer player))
            {
                player.Role = MMRole.Spectator;
            }
        }

        private void DroppingItem(DroppingItemEventArgs ev)
        {
            if (CustomItem.SerialItems.TryGetValue(ev.Item.Serial, out CustomItem item))
            {
                item.DroppingItem(ev);
            }
        }

        private void DroppingAmmo(DroppingAmmoEventArgs ev)
        {
            ev.IsAllowed = false;
            ev.Amount = 0;
        }

        private void PickingUpItem(PickingUpItemEventArgs ev)
        {
            if (CustomItem.SerialItems.TryGetValue(ev.Pickup.Serial, out CustomItem item))
            {
                item.PickingUpItem(ev);
            }
            else
            {
                ev.IsAllowed = false;
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

        private void Hurting(HurtingEventArgs ev)
        {
            if (MMPlayer.Get(ev.Attacker, out MMPlayer attacker))
            {
                if (MMPlayer.Get(ev.Target, out MMPlayer target))
                {
                    if (attacker.Role == MMRole.Detective)
                    {
                        ev.Amount *= 1.6f;
                    }

                    if (attacker.Role == target.Role)
                    {
                        ev.IsAllowed = false;
                    }
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

        private void GeneratorActivated(GeneratorActivatedEventArgs ev)
        {
            GeneratorsActivated++;
            ev.IsAllowed = true;
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
                    else
                    {
                        try
                        {
                            CustomItem.SerialItems.Add(item.Serial, CustomItem.Items[MMItem.UnprotectedItem]);
                        }
                        catch { }
                    }
                }


                try
                {
                    UnityEngine.Object.FindObjectOfType<Recontainer079>()._alreadyRecontained = true;
                }
                catch { }
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

                GamemodeCoroutines[0] = Timing.RunCoroutine(PlayerText());

                if (MurderMystery.Singleton.Config.RoundTime != 0)
                {
                    TimeUntilEnd = MurderMystery.Singleton.Config.RoundTime;
                    GamemodeCoroutines[1] = Timing.RunCoroutine(RoundTimer());
                }

                GamemodeCoroutines[2] = Timing.RunCoroutine(EquipmentTimer(MurderMystery.Singleton.Config.EquipmentDelay));
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
        
        private IEnumerator<float> PlayerText()
        {
            MMLog.Debug("[GamemodeManager::PlayerText]", "Primary function called.");

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
                    /*else // Will add custom timer for players to show the remaining time, or the remaining target count.
                    {
                        if (RoundTimerEnabled)
                        {

                        }
                        else
                        {

                        }
                    }*/
                }
            }
        }

        private IEnumerator<float> RoundTimer()
        {
            MMLog.Debug("[GamemodeManager::RoundTimer]", "Primary function called.");

            Config cfg = MurderMystery.Singleton.Config;

            while (GamemodeEnabled)
            {
                yield return Timing.WaitForOneFrame;
                TimeUntilEnd -= Timing.DeltaTime;

                if (!Murderers939Vision && TimeUntilEnd <= cfg.Murderers939VisionTime && cfg.Murderers939VisionTime != 0)
                {
                    MMLog.Debug("SCP-939 vision is being applied to murderers.");

                    List<MMPlayer> players = MMPlayer.List;

                    for (int i = 0; i < players.Count; i++)
                    {
                        if (players[i].Role == MMRole.Murderer)
                        {
                            players[i].Player.EnableEffect<CustomPlayerEffects.Visuals939>();
                            players[i].Player.Broadcast(5, "<b><size=30>You can now see players through walls.</size></b>");
                        }
                        else
                        {
                            players[i].Player.Broadcast(5, "<b><size=30>Murderers can now see players through walls.</size></b>");
                        }
                    }

                    Murderers939Vision = true;
                }

                if (!GeneratorsUnlocked && TimeUntilEnd <= cfg.GeneratorUnlockTime && cfg.GeneratorUnlockTime != 0)
                {
                    MMLog.Debug("Map generators are being unlocked.");

                    foreach (Scp079Generator generator in UnityEngine.Object.FindObjectsOfType<Scp079Generator>())
                    {
                        generator.Network_flags |= (byte)Scp079Generator.GeneratorFlags.Unlocked;
                    }

                    Map.Broadcast(15, "<size=40>Generators around the map have been unlocked.</size><size=30>\n<i>Note: If all generators are activated, <color=#ff0000>murderers win</color>.</i></size>");

                    GeneratorsUnlocked = true;
                }
            }
        }

        private IEnumerator<float> EquipmentTimer(float delay)
        {
            MMLog.Debug("[GamemodeManager::EquipmentTimer]", "Primary function called.");

            yield return Timing.WaitForSeconds(delay);

            MMLog.Debug("[GamemodeManager::EquipmentTimer]", "Giving players their equipment...");

            try
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

                MMLog.Debug("[GamemodeManager::EquipmentTimer]", "Player equipment has been given.");
            }
            catch (Exception e)
            {
                MMLog.Error("[GamemodeManager::EquipmentTimer]", e, "Failed to give player equipment.");

                Map.Broadcast(15, "<size=30>Murder Mystery gamemode failed to give player equipment. Round restart in 10 seconds.</size>");
                Timing.CallDelayed(10, () =>
                {
                    MurderMystery.Singleton.GamemodeManager.ToggleGamemode(false);
                    Round.Restart(false);
                });
            }
        }
        #endregion
    }
}
