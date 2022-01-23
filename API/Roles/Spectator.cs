using MurderMystery.API.Enums;
using MurderMystery.API.Features;

namespace MurderMystery.API.Roles
{
    public sealed class Spectator : CustomRole
    {
        public override string Name => "Spectator";
        public override string ColoredName => "<color=#7f7f7f>Spectator</color>";

        public override MMRole Role => MMRole.Spectator;
        public override string SpawnMsg => $"<size=70>You are a {ColoredName}</size>";
        public override string SpawnInfoMsg => "<b><size=50>You were detected in <color=#00f8fc>overwatch mode</color>.</size></b>";
    }
}
