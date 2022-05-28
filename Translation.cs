using Exiled.API.Interfaces;

namespace MurderMystery
{
    public sealed class Translation : ITranslation
    {
        public string JoinedMessage { get; set; } = "Murder Mystery gamemode is enabled for this round.";
        public string JoinedLateMessage { get; set; } = "Murder Mystery gamemode is currently active.";
    }
}
