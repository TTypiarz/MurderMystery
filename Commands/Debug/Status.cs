using CommandSystem;
using MurderMystery.API.Enums;
using System;

namespace MurderMystery.Commands.Debug
{
    public class Status : MMCommand
    {
        public override string Command => "status";

        public override string[] Aliases => new string[] { "stat" };

        public override string Description => "Shows the current status of the gamemode.";

        public override MMPerm Permission => MMPerm.None;

        public override bool PlayerOnly => false;

        public override bool ExecuteInternally(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            GamemodeManager gmm = MurderMystery.Singleton.GamemodeManager;

            response = $"\nEnabled: {(gmm.PrimaryEnabled ? "<color=#00ff00>YES</color>" : "<color=#ff0000>NO</color>")}\n" +
                $"Player Events enabled: {(gmm.PlayerEnabled ? "<color=#00ff00>YES</color>" : "<color=#ff0000>NO</color>")}\n" +
                $"Gamemode Events enabled: {(gmm.GamemodeEnabled ? "<color=#00ff00>YES</color>" : "<color=#ff0000>NO</color>")}\n\n" +
                $"RestartedRound: {(gmm.RestartedRound ? "<color=#00ff00>YES</color>" : "<color=#ff0000>NO</color>")}\n" +
                $"WaitingForPlayers: {(gmm.WaitingPlayers ? "<color=#00ff00>YES</color>" : "<color=#ff0000>NO</color>")}\n" +
                $"Started: {(gmm.Started ? "<color=#00ff00>YES</color>" : "<color=#ff0000>NO</color>")}\n" +
                $"Failed To Start: {(gmm.FailedToStart ? "<color=#00ff00>YES</color>" : "<color=#ff0000>NO</color>")}\n\n";

            response += "Coroutines:\n" +
                $"[0] PlayerText: {(gmm.GamemodeCoroutines[0].IsRunning ? "<color=#00ff00>RUNNING</color>" : "<color=#ff0000>NOT RUNNING</color>")}\n" +
                $"[1] RoundTimer: {(gmm.GamemodeCoroutines[1].IsRunning ? "<color=#00ff00>RUNNING</color>" : "<color=#ff0000>NOT RUNNING</color>")}\n" +
                $"[2] EquipmentTimer: {(gmm.GamemodeCoroutines[2].IsRunning ? "<color=#00ff00>RUNNING</color>" : "<color=#ff0000>NOT RUNNING</color>")}\n";

            return true;
        }
    }
}
