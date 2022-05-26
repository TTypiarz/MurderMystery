using Exiled.API.Enums;
using Exiled.API.Features;
using System;

namespace MurderMystery
{
    public class MurderMystery : Plugin<Config, Translation>
    {
        public override string Author => "Zereth";
        public override string Name => "MurderMystery";
        public override string Prefix => "murder_mystery";
        public override PluginPriority Priority => PluginPriority.Default;
        public override Version RequiredExiledVersion => new Version(5, 2, 1);
        public override Version Version => new Version(2, 0, 0);

        public override void OnEnabled()
        {
        }

        public override void OnDisabled()
        {
        }
    }
}
