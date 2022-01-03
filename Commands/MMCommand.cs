using CommandSystem;
using MurderMystery.API.Enums;
using MurderMystery.Extensions;
using RemoteAdmin;
using System;

namespace MurderMystery.Commands
{
    public abstract class MMCommand : ICommand
    {
        public abstract string Command { get; }

        public abstract string[] Aliases { get; }

        public abstract string Description { get; }

        public abstract MMPerm Permission { get; }

        public abstract bool PlayerOnly { get; }

        public abstract bool ExecuteInternally(ArraySegment<string> arguments, ICommandSender sender, out string response);

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender.CheckPermission(Permission))
            {
                if (PlayerOnly)
                {
                    if (sender is PlayerCommandSender)
                    {
                        return ExecuteInternally(arguments, sender, out response);
                    }
                    else
                    {
                        response = "This command can only be used by a player!";
                        return false;
                    }
                }
                else
                {
                    return ExecuteInternally(arguments, sender, out response);
                }
            }
            else
            {
                response = "You don't have permission to execute this command!\nMissing permission: " + Permission.ToPermissionString();
                return false;
            }
        }
    }
}
