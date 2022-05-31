using Exiled.API.Extensions;
using Exiled.Events.EventArgs;
using MurderMystery.API.Enums;
using MurderMystery.API.Internal;
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
            if (Plugin.MapPrepared && !Plugin.Started)
            {
                ev.Position = Plugin.Zone switch
                {
                    MMZone.LCZ => RoleType.Scp173.GetRandomSpawnProperties().Item1,
                    MMZone.HCZ => RoleType.Scp049.GetRandomSpawnProperties().Item1,
                    _ => throw new System.InvalidOperationException("Zone has not been set!")
                };
            }
        }

        private void ChangingRole(ChangingRoleEventArgs ev)
        {
            if (Plugin.MapPrepared && !Plugin.Started)
            {
                ev.NewRole = RoleType.ClassD;
                ev.Items.Clear();
                ev.Ammo.Clear();
            }
        }
    }
}
