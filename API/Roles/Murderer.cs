using MurderMystery.API.Enums;
using MurderMystery.API.Features;

namespace MurderMystery.API.Roles
{
    public sealed class Murderer : CustomRole
    {
        public override string Name => "Murderer";
        public override string ColoredName => "<color=#ff0000>Murderer</color>";

        public override MMRole Role => MMRole.Murderer;
        public override string SpawnMsg => $"<size=70>You are a {ColoredName}</size>";
        public override string SpawnInfoMsg => "<size=50>You must <color=#ff00ff>kill all</color> <color=#00ff00>innocents</color> and <color=#0000ff>detectives</color>.</size>";
    }
}
