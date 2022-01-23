using MurderMystery.API.Features;

namespace MurderMystery.API.Interfaces
{
    public interface IEquipment
    {
        string EquipmentMessage { get; }

        void GiveEquipment(MMPlayer player);
    }
}
