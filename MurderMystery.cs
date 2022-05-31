using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Firearms.Attachments;
using MapGeneration.Distributors;
using MEC;
using MurderMystery.API;
using MurderMystery.API.Enums;
using MurderMystery.API.Internal;
using MurderMystery.EventHandlers;
using System;
using System.Collections.Generic;

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
#if DEBUG
        public const bool InternalDebug = true;
#else
        public const bool InternalDebug = false;
#endif

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
            const DoorLockReason fullLock =
                DoorLockReason.AdminCommand
                | DoorLockReason.DecontLockdown
                | DoorLockReason.SpecialDoorFeature
                | DoorLockReason.NoPower
                | DoorLockReason.Lockdown2176;

            try
            {
                MMLog.Debug("Preparing map...");

                MMLog.Debug("Fixing workstations...");

                foreach (WorkstationController controller in WorkstationController.AllWorkstations)
                {
                    controller.NetworkStatus = 4;
                }

                MMLog.Debug("Fixing lockers...");

                foreach (Locker locker in UnityEngine.Object.FindObjectsOfType<Locker>())
                {
                    ushort num = 1;

                    for (int i = 0; i < locker.Chambers.Length; i++)
                    {
                        if (locker.Chambers[i].RequiredPermissions > 0)
                        {
                            locker.Chambers[i].SetDoor(true, null);
                            locker.NetworkOpenedChambers |= num;
                        }

                        num *= 2;
                    }
                }

                MMLog.Debug("Fixing doors...");

                foreach (Door door in Door.List)
                {
                    if (door.Base is CheckpointDoor chkDoor)
                    {
                        chkDoor.NetworkActiveLocks = (ushort)fullLock;
                        chkDoor.NetworkTargetState = false;
                        chkDoor.UsedBy106 = false;

                        for (int i = 0; i < chkDoor._subDoors.Length; i++)
                        {
                            DoorVariant subDoor = chkDoor._subDoors[i];
                            subDoor.NetworkActiveLocks = (ushort)fullLock;
                            subDoor.NetworkTargetState = false;
                            subDoor.UsedBy106 = false;
                        }
                    }
                    else if (door.RequiredPermissions.RequiredPermissions > 0)
                    {
                        door.Base.NetworkActiveLocks = (ushort)fullLock;
                        door.Base.NetworkTargetState = true;
                    }
                }

                MMLog.Debug("Fixing lifts...");

                foreach (Exiled.API.Features.Lift lift in Exiled.API.Features.Lift.List)
                {
                    switch (lift.Type)
                    {
                        case ElevatorType.LczA:
                        case ElevatorType.LczB:
                            lift.IsLocked = true;
                            continue;
                    }
                }

                MMLog.Debug("Fixing 079 recontainment...");

                UnityEngine.Object.FindObjectOfType<Recontainer079>()._alreadyRecontained = true;

                MapPrepared = true;

                MMLog.Debug("Map has been prepared.");
            }
            catch (Exception e)
            {
                DisableOnError(e, "Failed to prepare the map.");
            }
        }

        /// <summary>
        /// Called when the gamemode is starting. Sets up the gamemode.
        /// <para>Called after <see cref="PrepareMap"/></para>
        /// </summary>
        internal void StartGamemode()
        {
            foreach (MMPlayer player in MMPlayer._list)
            {
                player.SetRoleSilently(MMRole.Spectator); // placeholder for debugging

                player.CustomRole.OnFirstSpawn();
            }

            Started = true;
        }

        internal void DisableOnError(Exception e, string message)
        {
            MMLog.Error(string.Concat(
                "An error occured (",
                message,
                "):\n",
                e?.ToString() ?? "No exception."
            ));

            message = string.Concat(
                "<size=30>Murder mystery gamemode has been disabled. Round restart in 10 seconds.</size>\n<size=28><color=red>Error: ",
                string.IsNullOrEmpty(message) ? "Unspecified. (blame bad developer :troll:)" : message,
                "</color></size>"
            );

            DisableAndRestartWithMessage(message);
        }

        public void DisableAndRestartWithMessage(string message)
        {
            Map.Broadcast(300, message, Broadcast.BroadcastFlags.Normal, true);

            Timing.RunCoroutine(DisableCoroutine());
        }

        private IEnumerator<float> DisableCoroutine()
        {
            float t = 0;
            while ((PrimaryHandlers?.Enabled ?? false) && t < 10f)
            {
                yield return Timing.WaitForOneFrame;
                t += Timing.DeltaTime;
            }

            if (PrimaryHandlers?.Enabled ?? false)
            {
                ToggleGamemode(false);
                Round.Restart(false);
            }
        }

#endregion
    }
}
