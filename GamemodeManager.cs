using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;
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
                            }
                            else
                            {
                                Handlers.Server.WaitingForPlayers -= WaitingForPlayers;
                                Handlers.Server.RoundStarted -= RoundStarted;
                                Handlers.Server.RoundEnded -= RoundEnded;
                                Handlers.Server.RestartingRound -= RestartingRound;

                                MurderMystery.Singleton.Harmony.Unpatch(LateRoundStartPatch.Original, HarmonyLib.HarmonyPatchType.Postfix);
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
                                Handlers.Server.RespawningTeam += RespawningTeam;
                            }
                            else
                            {
                                Handlers.Player.ChangingRole -= ChangingRole;
                                Handlers.Player.Spawning -= Spawning;
                                Handlers.Server.RespawningTeam -= RespawningTeam;
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
            }
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
