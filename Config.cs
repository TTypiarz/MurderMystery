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

        [Description("Makes the gamemode always enabled.")]
        public bool AlwaysEnabled { get; set; } = false;


        [Description("Sets the percentage of murderers.")]
        public double MurdererPercentage { get; set; } = DefaultMurdererPercentage;
        public const double DefaultMurdererPercentage = 1d / 6d;

        [Description("Sets the murderer offset, simulates more players in the playercount when multiplying the percentage.")]
        public int MurdererOffset { get; set; } = DefaultMurdererOffset;
        public const int DefaultMurdererOffset = 0;

        [Description("Sets the percentage of detectives.")]
        public double DetectivePercentage { get; set; } = DefaultDetectivePercentage;
        public const double DefaultDetectivePercentage = 1d / 12d;

        [Description("Sets the detective offset, simulates more players in the playercount when multiplying the percentage.")]
        public int DetectiveOffset { get; set; } = DefaultDetectiveOffset;
        public const int DefaultDetectiveOffset = 6;


        internal void Validate()
        {
            // Validate config here.
        }
    }
}
