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
        public string ColoredName => string.Concat("<color=#", Color, ">", RoleName, "</color>");

        internal virtual void OnFirstSpawn()
        {
            if (MurderMystery.InternalDebug)
                MMLog.Debug(string.Concat("Player: '", Player.Player.Nickname, "' has spawned initially as: ", RoleName));

            // placeholder
            Player.Player.Broadcast(15, string.Concat("<size=30>You have spawned as: ", ColoredName, "</size>"));
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

            // placeholder
            Player.Player.Broadcast(5, string.Concat("<size=30>Your role has been changed to ", ColoredName, "</size>"));
        }

        public static MMCustomRole Create(MMPlayer player, MMRole value)
        {
            return value switch
            {
                MMRole.None => null,
                MMRole.Spectator => new Spectator(player),
                MMRole.Innocent => new Innocent(player),
                MMRole.Murderer => new Murderer(player),
                MMRole.Detective => new Detective(player),
                _ => throw new ArgumentOutOfRangeException("value", value, "Value must be defined."),
            };
        }
    }
}
