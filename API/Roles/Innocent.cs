using MurderMystery.API.Enums;
using MurderMystery.API.Features;

namespace MurderMystery.API.Roles
{
    public sealed class Innocent : CustomRole
    {
        public override string Name => "Innocent";
        public override string ColoredName => "<color=#00ff00>Innocent</color>";

        public override MMRole Role => MMRole.Innocent;
        public override string SpawnMsg => $"<size=70>You are {ColoredName}</size>";
        public override string SpawnInfoMsg => "<size=50>You must <color=#ff00ff>survive</color> and <color=#ff00ff>avoid</color> <color=#ff0000>murderers</color>.</size>";
    }
}
