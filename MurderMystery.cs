using Exiled.API.Enums;
using Exiled.API.Features;
using System;

namespace MurderMystery
{
    public class MurderMystery : Plugin<Config>
    {
        public override string Author => "Zereth";
        public override string Name => "MurderMystery";
        public override string Prefix => "murder_mystery";
        public override PluginPriority Priority => PluginPriority.Default;
        public override Version RequiredExiledVersion => new Version(3, 0, 0);
        public override Version Version => new Version(1, 0, 0);

        public static MurderMystery Singleton { get; private set; }

        public EventHandlers EventHandlers { get; private set; }

        public override void OnEnabled()
        {
            Singleton = this;
            EventHandlers = new EventHandlers(this);

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            EventHandlers = null;
            Singleton = null;

            base.OnDisabled();
        }
    }
}
