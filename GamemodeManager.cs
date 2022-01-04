using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using MurderMystery.API;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;
using MurderMystery.API.Interfaces;
using MurderMystery.Extensions;
using MurderMystery.Patches;
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

        public Random Rng { get; private set; } = new Random();
        public CoroutineHandle[] GamemodeCoroutines { get; } = new CoroutineHandle[1]
        {
            default // [0] SpectatorText
        };

        internal void ToggleGamemode(bool enable)
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

                    if (RespawnTimerPatch.PatchEnabled)
                    {
                        RespawnTimerPatch.TogglePatch(false);
                    }

                    Timing.KillCoroutines(GamemodeCoroutines);

                    Started = false;
                    WaitingPlayers = false;
                    FailedToStart = false;

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

                                MurderMystery.Singleton.Harmony.Patch(LateRoundStartPatch.Original, null, LateRoundStartPatch.Patch);
                                MurderMystery.Singleton.Harmony.Patch(RoundSummaryPatch.Original, RoundSummaryPatch.Patch);
                            }
                            else
                            {
                                Handlers.Server.WaitingForPlayers -= WaitingForPlayers;
                                Handlers.Server.RoundStarted -= RoundStarted;
                                Handlers.Server.RoundEnded -= RoundEnded;
                                Handlers.Server.RestartingRound -= RestartingRound;

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
                                Handlers.Server.RespawningTeam += RespawningTeam;
                                Handlers.Server.EndingRound += EndingRound;

                                ServerConsole.FriendlyFire = true;
                                FriendlyFireConfig.PauseDetector = true;
                                DependencyUtilities.HandleCedModV3(true);
                            }
                            else
                            {
                                Handlers.Player.ChangingRole -= ChangingRole;
                                Handlers.Player.Spawning -= Spawning;
                                Handlers.Player.Dying -= Dying;
                                Handlers.Player.DroppingItem -= DroppingItem;
                                Handlers.Server.RespawningTeam -= RespawningTeam;
                                Handlers.Server.EndingRound -= EndingRound;

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

                RespawnTimerPatch.TogglePatch(true);

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

                ev.Ammo.Clear();
                ev.Items.Clear();
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

            int innocents = MMPlayer.List.GetRolesCount(MMRole.Innocent, MMRole.Detective);
            int murderers = MMPlayer.List.GetRoleCount(MMRole.Murderer);

            if (innocents == 0 && murderers > 0)
            {
                Map.ClearBroadcasts();
                Map.Broadcast(300, "\n<size=80><color=#ff0000><b>Murderers win</b></color></size>\n<size=30>All innocents have been killed.</size>");
                goto Allow;
            }

            if (innocents > 0 && murderers == 0)
            {
                Map.ClearBroadcasts();
                Map.Broadcast(300, "\n<size=80><color=#00ff00><b>Innocents win</b></color></size>\n<size=30>All murderers have been killed.</size>");
                goto Allow;
            }

            if (innocents == 0 && murderers == 0)
            {
                Map.ClearBroadcasts();
                Map.Broadcast(300, "\n<size=80><color=#7f7f7f><b>Stalemate</b></color></size>\n<size=30>All players have been killed. HOW??? ( ͡° ͜ʖ ͡°)</size>");
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
                ply.Role = MMRole.Spectator;
            }
        }

        private void DroppingItem(DroppingItemEventArgs ev)
        {
            if (CustomItemPool.ProtectedItemIds.Contains(ev.Item.Serial))
                ev.IsAllowed = false;
        }

        #endregion

        #region Primary Functions
        private void StartGamemode()
        {
            try
            {
                MMLog.Debug("Primary function called.");

                // Make workstations un-usable to prevent attachment changes by players.
                foreach (WorkstationController controller in WorkstationController.AllWorkstations)
                {
                    controller.NetworkStatus = 4;
                }

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
                Map.ClearBroadcasts();
                Map.Broadcast(15, "<size=30>Murder Mystery gamemode failed to start. Round restart in 10 seconds.</size>");
                Timing.CallDelayed(10, () =>
                {
                    MurderMystery.Singleton.GamemodeManager.ToggleGamemode(false);
                    Round.Restart();
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
                    MMPlayer.List[i].CustomRole?.OnFirstSpawn(MMPlayer.List[i]);
                }

                GamemodeCoroutines[0] = Timing.RunCoroutine(SpectatorText());
            }
            catch (Exception e)
            {
                MMLog.Error($"FATAL ERROR:\n{e}");
                Map.ClearBroadcasts();
                Map.Broadcast(15, "<size=30>Murder Mystery gamemode failed to start. Round restart in 10 seconds.</size>");
                Timing.CallDelayed(10, () =>
                {
                    MurderMystery.Singleton.GamemodeManager.ToggleGamemode(false);
                    Round.Restart();
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
        #endregion
    }
}
