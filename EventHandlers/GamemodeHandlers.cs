using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs;
using MurderMystery.API.Enums;
using MurderMystery.API.Internal;
using System;
using Handlers = Exiled.Events.Handlers;

namespace MurderMystery.EventHandlers
{
    public sealed class GamemodeHandlers : MMEventHandler
    {
        public GamemodeHandlers(MurderMystery plugin) => Plugin = plugin;

        public override MurderMystery Plugin { get; }

        protected override void Enable()
        {
            Handlers.Player.Spawning += Spawning;
            Handlers.Player.ChangingRole += ChangingRole;
        }

        protected override void Disable()
        {
            Handlers.Player.Spawning -= Spawning;
            Handlers.Player.ChangingRole -= ChangingRole;
        }

        private void Spawning(SpawningEventArgs ev)
        {
            try
            {
                if (Plugin.SettingRoles && ev.RoleType == RoleType.ClassD)
                {
                    ev.Position = Plugin.Zone switch
                    {
                        MMZone.LCZ => RoleType.Scp173.GetRandomSpawnProperties().Item1,
                        MMZone.HCZ => RoleType.Scp049.GetRandomSpawnProperties().Item1,
                        _ => throw new InvalidOperationException("Zone has not been set!")
                    };
                    return;
                }
            }
            catch (Exception e)
            {
                MMLog.Error(e);
            }
        }

        private void ChangingRole(ChangingRoleEventArgs ev)
        {
            try
            {
                if (ev.NewRole != RoleType.Spectator)
                {
                    if (Plugin.SettingRoles)
                    {
                        ev.NewRole = RoleType.ClassD;
                        ev.Items.Clear();
                        ev.Ammo.Clear();
                    }
                    else if (ev.Reason == SpawnReason.LateJoin)
                    {
                        ev.NewRole = RoleType.Spectator;
                        ev.Items.Clear();
                        ev.Ammo.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                MMLog.Error(e);
            }
        }


    }
}
