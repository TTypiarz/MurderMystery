using MurderMystery.API;

namespace MurderMystery.Handlers
{
    public sealed class PlayerHandlers : MMEventHandler
    {
        public PlayerHandlers(MurderMystery plugin) => Plugin = plugin;

        public override MurderMystery Plugin { get; }

        protected override void Enable()
        {
        }

        protected override void Disable()
        {
        }
    }
}
