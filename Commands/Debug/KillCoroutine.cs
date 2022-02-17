using CommandSystem;
using MEC;
using MurderMystery.API.Enums;
using System;

namespace MurderMystery.Commands.Debug
{
    public class KillCoroutine : MMCommand
    {
        public override string Command => "killcoroutine";

        public override string[] Aliases => new string[] { "kc" };

        public override string Description => "Kills a coroutine at the specified index of the main gamemode coroutines array.";

        public override MMPerm Permission => MMPerm.Debug;

        public override bool PlayerOnly => false;

        public override bool ExecuteInternally(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string[] args = arguments.Array;

            if (args.Length < 3)
            {
                response = "You must supply an index.";
                return false;
            }

            if (int.TryParse(args[2], out int index))
            {
                if (MurderMystery.Singleton.GamemodeManager.GamemodeCoroutines.Length > index && index >= 0)
                {
                    Timing.KillCoroutines(MurderMystery.Singleton.GamemodeManager.GamemodeCoroutines[index]);
                    response = "Coroutine at index [" + index + "] was killed. This may cause instability!";
                    return true;
                }
                else
                {
                    response = "Index supplied was invalid.";
                    return false;
                }
            }
            else
            {
                response = "Index supplied was invalid!";
                return false;
            }
        }
    }
}
