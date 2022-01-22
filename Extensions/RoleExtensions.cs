using MurderMystery.API.Enums;
using MurderMystery.API.Features;
using System.Collections.Generic;
using System.Linq;

namespace MurderMystery.Extensions
{
    public static class RoleExtensions
    {
        public static List<MMPlayer> PlayersCanView(this CustomRole role)
        {
            return MMPlayer.List.GetRoles(role.RolesCanView);
        }

        public static List<MMPlayer> CanSeePlayers(this CustomRole role)
        {
            return MMPlayer.List.GetRoles(role.CanSeeRoles());
        }

        public static MMRole[] CanSeeRoles(this CustomRole role)
        {
            return CustomRole.Roles.Values.Where(x => x != null && x.RolesCanView.Contains(role.Role)).Select(x => x.Role).ToArray();
        }
    }
}
