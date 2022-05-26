using MurderMystery.API;

namespace MurderMystery.Handlers
{
    public sealed class GamemodeHandlers : MMEventHandler
    {
        public GamemodeHandlers(MurderMystery plugin) => Plugin = plugin;

        public override MurderMystery Plugin { get; }

        protected override void Enable()
        {
        }

        protected override void Disable()
        {
        }
    }
}
