using MurderMystery.API.Enums;
using System;

namespace MurderMystery.Extensions
{
    public static class EnumExtensions
    {
        public static string ToPermissionString(this MMPerm perm)
        {
            switch (perm)
            {
                case MMPerm.Enable:
                    return "murdermystery.enable";
                case MMPerm.Debug:
                    return "murdermystery.debug";
                case MMPerm.ShowRoles:
                    return "murdermystery.showroles";
                case MMPerm.SetRoles:
                    return "murdermystery.setroles";

                default:
                    throw new ArgumentException(nameof(perm));
            }
        }
    }
}
