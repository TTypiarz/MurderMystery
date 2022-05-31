using Exiled.Events.EventArgs;
using MurderMystery.API.Enums;
using MurderMystery.API.Internal;
using MurderMystery.Patches;
using System;
using Handlers = Exiled.Events.Handlers;

namespace MurderMystery.EventHandlers
{
    public sealed class PrimaryHandlers : MMEventHandler
    {
        public PrimaryHandlers(MurderMystery plugin) => Plugin = plugin;

        public override MurderMystery Plugin { get; }

        protected override void Enable()
        {
            Handlers.Server.WaitingForPlayers += WaitingForPlayers;
            Handlers.Server.RoundStarted += RoundStarted;
            LateRoundStartPatch.LateRoundStarted += LateRoundStarted;
            Handlers.Server.RoundEnded += RoundEnded;
            Handlers.Server.RestartingRound += RestartingRound;

            Plugin.Zone = MMZone.None;
        }

        protected override void Disable()
        {
            Handlers.Server.WaitingForPlayers -= WaitingForPlayers;
            Handlers.Server.RoundStarted -= RoundStarted;
            LateRoundStartPatch.LateRoundStarted -= LateRoundStarted;
            Handlers.Server.RoundEnded -= RoundEnded;
            Handlers.Server.RestartingRound -= RestartingRound;

            Plugin.Zone = MMZone.None;
        }

        private void WaitingForPlayers()
        {
            try
            {
                if (!Plugin.PlayerHandlers.Enabled)
                {
                    MMLog.Debug("Enabling player events.");

                    Plugin.PlayerHandlers.ToggleHandlers(true);

                    Plugin.WaitingPlayers = true;
                }
            }
            catch (Exception e)
            {
                MMLog.Error(e);
            }
        }

        private void RoundStarted()
        {
            try
            {
                if (Plugin.WaitingPlayers)
                {
                    MMLog.Debug("Enabling gamemode events.");

                    Plugin.GamemodeHandlers.ToggleHandlers(true);

                    Plugin.PrepareMap();
                }
            }
            catch (Exception e)
            {
                MMLog.Error(e);
            }
        }

        private void LateRoundStarted()
        {
            try
            {
                if (Plugin.MapPrepared)
                {
                    MMLog.Debug("Starting gamemode.");

                    Plugin.StartGamemode();
                }
            }
            catch (Exception e)
            {
                MMLog.Error(e);
            }
        }

        private void RoundEnded(RoundEndedEventArgs ev)
        {
            try
            {
                if (Plugin.GamemodeHandlers.Enabled)
                {
                    MMLog.Debug("Disabling gamemode events.");

                    Plugin.GamemodeHandlers.ToggleHandlers(false);
                }
            }
            catch (Exception e)
            {
                MMLog.Error(e);
            }
        }

        private void RestartingRound()
        {
            try
            {
                if (Plugin.Started)
                {
                    MMLog.Debug("Disabling gamemode.");

                    Plugin.ToggleGamemode(false);

                    if (MurderMystery.Singleton.Config.AlwaysEnabled)
                    {
                        MMLog.Debug("Re-enabling gamemode. 'always_enabled' is set to true");

                        Plugin.ToggleGamemode(true);
                    }
                }
                else
                    Plugin.MapPrepared = false;
            }
            catch (Exception e)
            {
                MMLog.Error(e);
            }
        }
    }
}
