using Exiled.API.Interfaces;

namespace MurderMystery
{
    public sealed class Translation : ITranslation
    {
        public string JoinedMessage { get; set; } = "Murder Mystery gamemode is enabled for this round.";
        public string JoinedLateMessage { get; set; } = "Murder Mystery gamemode is currently active.";

        public string SpectatorRoleName { get; set; } = "Spectator";
        public string InnocentRoleName { get; set; } = "Innocent";
        public string MurdererRoleName { get; set; } = "Murderer";
        public string DetectiveRoleName { get; set; } = "Detective";
    }
}
