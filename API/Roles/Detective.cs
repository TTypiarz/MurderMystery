using MurderMystery.API.Enums;
using MurderMystery.API.Features;

namespace MurderMystery.API.Roles
{
    public sealed class Detective : CustomRole
    {
        public override string Name => "Detective";
        public override string ColoredName => "<color=#0000ff>Detective</color>";

        public override MMRole Role => MMRole.Detective;
        public override string SpawnMsg => $"<size=70>You are a {ColoredName}</size>";
        public override string SpawnInfoMsg => "<size=50>You must <color=#ff00ff>protect</color> <color=#ff0000>innocents</color> and <color=#ff00ff>kill</color> <color=#ff0000>murderers</color>.</size>";
    }
}
