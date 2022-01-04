using Exiled.API.Interfaces;
using MurderMystery.API.Features;
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

        [Description("Sets the percentage of murderers.")]
        public decimal MurdererPercentage { get; set; } = DefaultMurdererPercentage;

        [Description("Sets the murderer offset, simulates more players in the playercount when multiplying the percentage.")]
        public int MurdererOffset { get; set; } = DefaultMurdererOffset;

        [Description("Sets the percentage of detectives.")]
        public decimal DetectivePercentage { get; set; } = DefaultDetectivePercentage;

        [Description("Sets the detective offset, simulates more players in the playercount when multiplying the percentage.")]
        public int DetectiveOffset { get; set; } = DefaultDetectiveOffset;

        [Description("The delay from the start of the round (in seconds) before equipment is given to players.")]
        public int EquipmentDelay { get; set; } = DefaultEquipmentDelay;

        public const decimal DefaultMurdererPercentage = 1m / 6m;
        public const int DefaultMurdererOffset = 0;
        public const decimal DefaultDetectivePercentage = 1m / 12m;
        public const int DefaultDetectiveOffset = 6;
        public const int DefaultEquipmentDelay = 45;

        internal void Validate()
        {
            if (MurdererPercentage + DetectivePercentage > 1m)
            {
                MMLog.Warn("Murderer and detective percentage configs sum to a value greater than 1. Using default configs...");
                goto UseDefault;
            }

            if (MurdererPercentage <= 0)
            {
                MMLog.Warn("Murderer percentage cannot be less than or equal to zero. Using default configs...");
                goto UseDefault;
            }
            if (DetectivePercentage <= 0)
            {
                MMLog.Warn("Detective percentage cannot be less than or equal to zero. Using default configs...");
                goto UseDefault;
            }

            if (MurdererOffset < 0)
            {
                MMLog.Warn("Murderer offset is less than zero, setting to zero.");
                MurdererOffset = 0;
            }
            if (DetectiveOffset < 0)
            {
                MMLog.Warn("Detective offset is less than zero, setting to zero.");
                MurdererOffset = 0;
            }

            for (int i = 0; i < 101; i++)
            {
                if (!CalculateRoles(i, out int m, out int d))
                {
                    MMLog.Warn("An issue was detected in your configuration when testing simulated player counts:\n" +
                        $"Simulated player count: {i}, Murderer output: {m}, Detective output: {d}\n" +
                        "Default configuration will be used.");
                    goto UseDefault;
                }
            }

            MMLog.Info("Config has been validated.");
            return;

        UseDefault:
            MurdererPercentage = DefaultMurdererPercentage;
            MurdererOffset = DefaultMurdererOffset;
            DetectivePercentage = DefaultDetectivePercentage;
            DetectiveOffset = DefaultDetectiveOffset;

            MMLog.Info("Config has been validated.");
        }

        /// <summary>
        /// Calculates the number of each role given the current configuration and a player count.
        /// </summary>
        /// <param name="playercount"></param>
        /// <param name="murderers"></param>
        /// <param name="detectives"></param>
        /// <returns>A bool specifying if the output values are valid.</returns>
        public bool CalculateRoles(int playercount, out int murderers, out int detectives)
        {
            if (playercount < 3)
            {
                murderers = playercount > 0 ? 1 : 0;
                detectives = playercount == 2 ? 1 : 0;
                return true;
            }

            if (1m / MurdererPercentage > playercount)
            {
                murderers = 1;
            }
            else
            {
                murderers = (int)Math.Floor(MurdererPercentage * (playercount + MurdererOffset));
            }

            if (1m / DetectivePercentage > playercount)
            {
                detectives = 1;
            }
            else
            {
                detectives = (int)Math.Floor(DetectivePercentage * (playercount + DetectiveOffset));
            }

            return murderers + detectives <= playercount;
        }
    }
}
