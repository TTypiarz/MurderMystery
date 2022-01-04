using Exiled.API.Features;
using InventorySystem.Items.Firearms;
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
        public override string SpawnInfoMsg => "<size=50>You must <color=#ff00ff>protect</color> <color=#00ff00>innocents</color> and <color=#ff00ff>kill</color> <color=#ff0000>murderers</color>.</size>";

        internal override void OnFirstSpawn(MMPlayer player)
        {
            Firearm gun = Server.Host.Inventory.CreateItemInstance(ItemType.GunCOM18, false) as Firearm;
            gun.Status = new FirearmStatus(gun.Status.Ammo, FirearmStatusFlags.Cocked | FirearmStatusFlags.Chambered | FirearmStatusFlags.MagazineInserted, 553);

            player.Player.Inventory.UserInventory.Items[gun.ItemSerial] = gun;
            player.Player.Inventory.SendItemsNextFrame = true;

            base.OnFirstSpawn(player);
        }
    }
}
