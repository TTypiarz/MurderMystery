using MurderMystery.API.Enums;
using MurderMystery.API.Features;
using MurderMystery.API.Interfaces;

namespace MurderMystery.API.Roles
{
    public sealed class Innocent : CustomRole, IEquipment
    {
        public override string Name => "Innocent";
        public override string ColoredName => "<color=#00ff00>Innocent</color>";

        public override MMRole Role => MMRole.Innocent;
        public override string SpawnMsg => $"<size=70>You are {ColoredName}</size>";
        public override string SpawnInfoMsg => "<b><size=50>You must <color=#ff00ff>survive</color> and <color=#ff00ff>avoid</color> <color=#ff0000>murderers</color>.</size></b>";

        public string EquipmentMessage { get; } = "<color=#00ff00><size=30>You have recieved your equipment.</size></color>";

        public void GiveEquipment(MMPlayer player)
        {
            CustomItem.SerialItems.Add(player.Player.AddItem(ItemType.Painkillers).Serial, CustomItem.Items[MMItem.UnprotectedItem]);
        }
    }
}
