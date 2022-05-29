using MurderMystery.API.Enums;

namespace MurderMystery.API.Roles
{
    public class Spectator : MMCustomRole
    {
        internal Spectator(MMPlayer player) => Player = player;

        public override MMPlayer Player { get; }
        public override MMRole Role => MMRole.Spectator;
        public override string RoleName => MurderMystery.Singleton.Translation.SpectatorRoleName;
        public override string Color => "808080";
    }
}
