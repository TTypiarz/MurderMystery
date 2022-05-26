using MurderMystery.API;

namespace MurderMystery.Handlers
{
    public sealed class PrimaryHandlers : MMEventHandler
    {
        public PrimaryHandlers(MurderMystery plugin) => Plugin = plugin;

        public override MurderMystery Plugin { get; }

        protected override void Enable()
        {
        }

        protected override void Disable()
        {
        }
    }
}
