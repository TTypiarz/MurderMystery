using MurderMystery.API.Enums;
using MurderMystery.API.Internal;
using MurderMystery.API.Roles;
using System;

namespace MurderMystery.API
{
    public abstract class MMCustomRole
    {
        protected MMCustomRole() { }

        public abstract MMPlayer Player { get; }
        public abstract MMRole Role { get; }
        public abstract string RoleName { get; }
        public abstract string Color { get; }

        internal virtual void OnFirstSpawn()
        {
            if (MurderMystery.InternalDebug)
                MMLog.Debug(string.Concat("Player: '", Player.Player.Nickname, "' has spawned initially as: ", RoleName));
        }
        internal virtual void ChangedRole(MMCustomRole oldRole)
        {
            if (MurderMystery.InternalDebug)
                MMLog.Debug(string.Concat("Player: '", Player.Player.Nickname, "' changed roles: ", RoleName));
        }
        internal virtual void ChangingRole(MMCustomRole newRole)
        {
            if (MurderMystery.InternalDebug)
                MMLog.Debug(string.Concat("Player: '", Player.Player.Nickname, "' is changing roles: ", RoleName));
        }

        public static MMCustomRole Create(MMPlayer player, MMRole value)
        {
            return value switch
            {
                MMRole.None => null,
                MMRole.Spectator => new Spectator(player),
                _ => throw new ArgumentOutOfRangeException("value", value, "Value must be defined."),
            };
        }
    }
}
