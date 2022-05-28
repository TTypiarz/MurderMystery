using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;
using MurderMystery.API;
using MurderMystery.EventHandlers;
using System;

namespace MurderMystery
{
    /// <summary>
    /// The main plugin class of the assembly.
    /// </summary>
    public class MurderMystery : Plugin<Config, Translation>
    {
        static MurderMystery()
        {
            Harmony = new Harmony("zereth.plugins.murdermystery");
            Harmony.PatchAll();
        }

        public override string Author => "Zereth";
        public override string Name => "MurderMystery";
        public override string Prefix => "murder_mystery";
        public override PluginPriority Priority => PluginPriority.Default;
        public override Version RequiredExiledVersion => new Version(5, 2, 1);
        public override Version Version => new Version(2, 0, 0);

        /// <summary>
        /// The harmony instance used by the plugin.
        /// </summary>
        public static Harmony Harmony { get; }

        /// <summary>
        /// The singleton instance of this plugin class.
        /// </summary>
        public static MurderMystery Singleton { get; private set; }

        /// <summary>
        /// A constant used to specify if the current build is for debug purposes. Mainly for compiler purposes.
        /// </summary>
        public const bool InternalDebug = true;

        public override void OnEnabled()
        {
            Singleton = this;

            PrimaryHandlers = new PrimaryHandlers(this);
            PlayerHandlers = new PlayerHandlers(this);
            GamemodeHandlers = new GamemodeHandlers(this);

            Config.Validate();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            PrimaryHandlers.ToggleHandlers(false);
            PrimaryHandlers = null;
            PlayerHandlers.ToggleHandlers(false);
            PlayerHandlers = null;
            GamemodeHandlers.ToggleHandlers(false);
            GamemodeHandlers = null;

            Singleton = null;

            base.OnDisabled();
        }

        /// <summary>
        /// A method that sepcifies if debug logging is enabled.
        /// </summary>
        /// <returns>A <see cref="bool"/> specifying if debug logs should be allowed for the plugin.</returns>
        public static bool AllowDebug()
        {
            return InternalDebug || (Singleton?.Config.Debug ?? false);
        }

        #region Gamemode Manager

        public PrimaryHandlers PrimaryHandlers { get; private set; }
        public PlayerHandlers PlayerHandlers { get; private set; }
        public GamemodeHandlers GamemodeHandlers { get; private set; }

        /// <summary>
        /// Specifies if the gamemode is enabled and waiting for players.
        /// </summary>
        public bool WaitingPlayers { get; internal set; }
        /// <summary>
        /// Specifies if the gamemdoe is enabled and the map has been prepared.
        /// </summary>
        public bool MapPrepared { get; internal set; }
        /// <summary>
        /// Specifies if the gamemode is enabled and has started.
        /// </summary>
        public bool Started { get; internal set; }

        /// <summary>
        /// Toggles the gamemode to the specified value, if possible.
        /// </summary>
        /// <param name="enable">Specifies whether the gamemode should be enabled.</param>
        public void ToggleGamemode(bool enable)
        {
            if (PrimaryHandlers.Enabled ^ enable)
            {
                if (enable)
                {
                    PrimaryHandlers.ToggleHandlers(true);
                }
                else
                {
                    PrimaryHandlers.ToggleHandlers(false);
                    PlayerHandlers.ToggleHandlers(false);
                    GamemodeHandlers.ToggleHandlers(false);

                    ResetValues();
                }
            }
        }

        /// <summary>
        /// Resets class instance values to their defaults.
        /// </summary>
        private void ResetValues()
        {
            WaitingPlayers = false;
            MapPrepared = false;
            Started = false;
        }

        /// <summary>
        /// Called when the map is being prepared. Prepares the map for the gamemode.
        /// <para>Called before <see cref="StartGamemode"/></para>
        /// </summary>
        internal void PrepareMap()
        {
            MapPrepared = true;
        }

        /// <summary>
        /// Called when the gamemode is starting. Sets up the gamemode.
        /// <para>Called after <see cref="PrepareMap"/></para>
        /// </summary>
        internal void StartGamemode()
        {
            Started = true;
        }

        #endregion
    }
}
