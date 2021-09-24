using CommandSystem;
using Exiled.Permissions.Extensions;
using MurderMystery.API.Enums;

namespace MurderMystery.Extensions
{
    public static class PlayerExtensions
    {
        public static bool CheckPermission(this ICommandSender sender, MMPerm perm)
        {
            if (perm != MMPerm.None)
            {
                return sender.CheckPermission(perm.ToPermissionString());
            }
            else
            {
                return true;
            }
        }
    }
}
