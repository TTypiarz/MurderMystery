using Exiled.API.Interfaces;
using System.ComponentModel;

namespace MurderMystery
{
    public sealed class Config : IConfig
    {
        [Description("Enables the murder mystery plugin.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enables debug logging for release versions.")]
        public bool Debug { get; set; } = false;

        internal void Validate()
        {
            MurderMystery.Info("Config has been validated.");
        }
    }
}
