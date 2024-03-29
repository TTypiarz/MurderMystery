﻿using Exiled.Events.EventArgs;
using GameCore;
using LightContainmentZoneDecontamination;
using MurderMystery.API;
using MurderMystery.API.Internal;
using System;
using System.Collections.Generic;
using Handlers = Exiled.Events.Handlers;

namespace MurderMystery.EventHandlers
{
    public sealed class PlayerHandlers : MMEventHandler
    {
        public PlayerHandlers(MurderMystery plugin) => Plugin = plugin;

        public override MurderMystery Plugin { get; }

        protected override void Enable()
        {
            MMPlayer._list = new List<MMPlayer>();

            Handlers.Player.Verified += Verified;
            Handlers.Player.Destroying += Destroying;

            if (DecontaminationController.Singleton != null)
                DecontaminationController.Singleton.disableDecontamination = true;
        }

        protected override void Disable()
        {
            MMPlayer._list = null;

            Handlers.Player.Verified -= Verified;
            Handlers.Player.Destroying -= Destroying;

            if (DecontaminationController.Singleton != null)
                DecontaminationController.Singleton.disableDecontamination = ConfigFile.ServerConfig.GetBool("disable_decontamination");
        }

        private void Verified(VerifiedEventArgs ev)
        {
            try
            {
                MMPlayer player = new MMPlayer(ev.Player);

                player.Verified();

                MMPlayer._list.Add(player);

                MMLog.Debug("Player verified. (Added to list)");
            }
            catch (Exception e)
            {
                MMLog.Error(e);
            }
        }

        private void Destroying(DestroyingEventArgs ev)
        {
            try
            {
                if (MMPlayer.TryGet(ev.Player, out MMPlayer player))
                {
                    player.Destroying();

                    MMPlayer._list.Remove(player);

                    MMLog.Debug("Player destroying. (Removed from list)");
                }
            }
            catch (Exception e)
            {
                MMLog.Error(e);
            }
        }
    }
}
