using Exiled.API.Features;
using MEC;
using MurderMystery.API.Enums;
using System.Collections.Generic;

namespace MurderMystery.API
{
    public class MMPlayer
    {
        internal MMPlayer(Player player) => _player = player;


        private readonly Player _player;
        public Player Player => _player;

        private MMCustomRole _role;
        public MMCustomRole CustomRole => _role;
        public MMRole Role
        {
            get => _role?.Role ?? MMRole.None;
            set
            {
                if ((_role?.Role ?? MMRole.None) != value)
                {
                    MMCustomRole newRole = MMCustomRole.Create(this, value);

                    _role?.ChangingRole(newRole);

                    MMCustomRole oldRole = _role;

                    _role = newRole;

                    newRole?.ChangedRole(oldRole);
                }
            }
        }


        internal static List<MMPlayer> _list;
        public static MMPlayer[] List => _list?.ToArray() ?? new MMPlayer[0];


        internal void Verified()
        {
            if (MurderMystery.Singleton.Started)
            {
                Player.Broadcast(15, string.Concat("<size=30>", MurderMystery.Singleton.Translation.JoinedLateMessage, "</size>"));
            }
            else
            {
                Player.Broadcast(15, string.Concat("<size=30>", MurderMystery.Singleton.Translation.JoinedMessage, "</size>"));
            }
        }

        internal void Destroying()
        {
        }

        public void SetRoleSilently(MMRole value)
        {
            if ((_role?.Role ?? MMRole.None) != value)
                _role = MMCustomRole.Create(this, value);
        }


        public static bool TryGet(Player player, out MMPlayer mmplayer)
        {
            return (mmplayer = Get(player)) != null;
        }

        public static MMPlayer Get(Player player)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i]._player == player)
                {
                    return _list[i];
                }
            }

            return null;
        }
    }
}
