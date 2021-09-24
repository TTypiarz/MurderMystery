using Exiled.Events.EventArgs;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;
using System;
using Handlers = Exiled.Events.Handlers;

namespace MurderMystery
{
    public class EventHandlers
    {
        internal EventHandlers(MurderMystery murderMystery) => MurderMystery = murderMystery;
        private readonly MurderMystery MurderMystery;

        public bool PrimaryEnabled { get; private set; } = false;
        public bool PlayerEnabled { get; private set; } = false;
        public bool GamemodeEnabled { get; private set; } = false;

        public bool Started { get; internal set; }

        internal void ToggleEvent(MMEventType eventType, bool enable)
        {
            try
            {
                MurderMystery.Debug($"{(enable ? "Enabling" : "Disabling")} {eventType} events.");

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
                            }
                            else
                            {
                                Handlers.Server.WaitingForPlayers -= WaitingForPlayers;
                                Handlers.Server.RoundStarted -= RoundStarted;
                                Handlers.Server.RoundEnded -= RoundEnded;
                                Handlers.Server.RestartingRound -= RestartingRound;
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

                            }
                            else
                            {

                            }

                            GamemodeEnabled = enable;
                        }
                        return;
                }
            }
            catch (Exception e)
            {
                MurderMystery.Error($"FATAL ERROR:\n{e}");
            }
        }

        #region Primary Events

        private void WaitingForPlayers()
        {
            if (!PlayerEnabled)
            {
                MurderMystery.Debug("Primary event called. Enabling player events...");

                ToggleEvent(MMEventType.Player, true);

                MurderMystery.RoundStartPatch.Patch(true);
            }
        }

        private void RoundStarted()
        {
            if (GamemodeEnabled)
            {
                MurderMystery.Debug("Primary event called. Starting gamemode...");

                MurderMystery.StartGamemode();

                Started = true;
            }
        }

        private void RoundEnded(RoundEndedEventArgs ev)
        {
            if (GamemodeEnabled)
            {
                MurderMystery.Debug("Primary event called. Disabling gamemode events...");

                ToggleEvent(MMEventType.Gamemode, false);
            }
        }

        private void RestartingRound()
        {
            if (Started)
            {
                MurderMystery.Debug("Primary event called. Disabling gamemode...");

                MurderMystery.ToggleGamemode(false);
            }
        }

        #endregion

        #region Player Events

        private void Verified(VerifiedEventArgs ev)
        {
            MMPlayer player = new MMPlayer(ev.Player);

            player.Verified();

            MMPlayer.List.Add(player);

            MurderMystery.Debug("Player verified. (Added to list)");
        }

        private void Destroying(DestroyingEventArgs ev)
        {
            MMPlayer player = MMPlayer.Get(ev.Player);

            player?.Destroying();

            MMPlayer.List.Remove(player);

            MurderMystery.Debug("Player destroying. (Removed from list)");
        }

        #endregion
    }
}
