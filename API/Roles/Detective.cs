using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;
using MurderMystery.API.Interfaces;
using MurderMystery.Extensions;

namespace MurderMystery.API.Roles
{
    public sealed class Detective : CustomRole, IEquipment
    {
        public override string Name => "Detective";
        public override string ColoredName => "<color=#0000ff>Detective</color>";

        public override MMRole Role => MMRole.Detective;
        public override string SpawnMsg => $"<size=70>You are a {ColoredName}</size>";
        public override string SpawnInfoMsg => "<b><size=50>You must <color=#ff00ff>protect</color> <color=#00ff00>innocents</color> and <color=#ff00ff>kill</color> <color=#ff0000>murderers</color>.</size></b>";

        public override MMRole[] RolesCanView { get; } = new MMRole[] { MMRole.Spectator, MMRole.Detective };

        public string EquipmentMessage { get; } = "<color=#0000ff><size=30>You have recieved your equipment.</size></color>";

        internal override void OnFirstSpawn(MMPlayer player)
        {
            base.OnFirstSpawn(player);

            player.InnocentKills = 0;
        }

        protected override void ChangingMMRole(MMPlayer player, CustomRole newRole)
        {
            player.InnocentKills = 0;
        }

        protected override void ChangedMMRole(MMPlayer player, CustomRole oldRole)
        {
            player.InnocentKills = 0;
        }

        public void GiveEquipment(MMPlayer player)
        {
            player.RemoveInvalidItems();

            ItemBase weapon = player.Player.AddItem(ItemType.GunRevolver).Base;
            (weapon as Firearm).Status = new FirearmStatus(6, FirearmStatusFlags.MagazineInserted, 553);
            CustomItem.SerialItems.Add(weapon.ItemSerial, CustomItem.Items[MMItem.DetectiveWeapon]);
            CustomItem.SerialItems.Add(player.Player.AddItem(ItemType.Medkit).Serial, CustomItem.Items[MMItem.UnprotectedItem]);
            CustomItem.SerialItems.Add(player.Player.AddItem(ItemType.Painkillers).Serial, CustomItem.Items[MMItem.UnprotectedItem]);
        }
    }
}
