using MurderMystery.API.Enums;

namespace MurderMystery.API.Roles
{
    public class Detective : MMCustomRole
    {
        internal Detective(MMPlayer player) => Player = player;

        public override MMPlayer Player { get; }
        public override MMRole Role => MMRole.Detective;
        public override string RoleName => MurderMystery.Singleton.Translation.DetectiveRoleName;
        public override string Color => "0000FF";
    }
}
