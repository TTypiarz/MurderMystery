using Exiled.Events.EventArgs;
using MurderMystery.API.Enums;
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

        internal void ToggleEvent(MMEventType eventType, bool enable)
        {
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
                    }
                    return;


                case MMEventType.Gamemode:
                    return;
            }
        }

        #region Primary Events

        private void WaitingForPlayers()
        {

        }

        private void RoundStarted()
        {

        }

        private void RoundEnded(RoundEndedEventArgs ev)
        {

        }

        private void RestartingRound()
        {

        }

        #endregion

        #region Player Events

        private void Verified(VerifiedEventArgs ev)
        {

        }

        private void Destroying(DestroyingEventArgs ev)
        {

        }

        #endregion
    }
}
