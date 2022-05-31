using MurderMystery.API.Enums;

namespace MurderMystery.API.Roles
{
    public class Murderer : MMCustomRole
    {
        internal Murderer(MMPlayer player) => Player = player;

        public override MMPlayer Player { get; }
        public override MMRole Role => MMRole.Murderer;
        public override string RoleName => MurderMystery.Singleton.Translation.MurdererRoleName;
        public override string Color => "FF0000";
    }
}
