using Exiled.API.Interfaces;
using System;
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

        public bool CalculateRoles(int playercount, out int murderers, out int detectives)
        {
            if (playercount < 3)
            {
                murderers = playercount > 0 ? 1 : 0;
                detectives = playercount == 2 ? 1 : 0;
                return true;
            }

            if (1d / MurdererPercentage > playercount)
            {
                murderers = 1;
            }
            else
            {
                murderers = (int)Math.Floor(MurdererPercentage * (playercount + MurdererOffset));
            }

            if (1d / DetectivePercentage > playercount)
            {
                detectives = 1;
            }
            else
            {
                detectives = (int)Math.Floor(DetectivePercentage * (playercount + DetectiveOffset));
            }

            if (MurdererPercentage + DetectivePercentage == 1d && murderers + detectives != playercount)
            {
                switch (new Random().Next(2))
                {
                    case 0:
                        murderers++;
                        break;

                    case 1:
                        detectives++;
                        break;
                }
            }

            return murderers + detectives <= playercount;
        }
    }
}
