using MurderMystery.API.Enums;

namespace MurderMystery.API.Roles
{
    public class Innocent : MMCustomRole
    {
        internal Innocent(MMPlayer player) => Player = player;

        public override MMPlayer Player { get; }
        public override MMRole Role => MMRole.Innocent;
        public override string RoleName => MurderMystery.Singleton.Translation.InnocentRoleName;
        public override string Color => "00FF00";
    }
}
