using Exiled.API.Extensions;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;
using MurderMystery.API.Interfaces;
using MurderMystery.Extensions;
using System.Collections.Generic;

namespace MurderMystery.API.Roles
{
    public sealed class Murderer : CustomRole, IEquipment
    {
        public override string Name => "Murderer";
        public override string ColoredName => "<color=#ff0000>Murderer</color>";
        public override Misc.PlayerInfoColorTypes PlayerInfoColor => Misc.PlayerInfoColorTypes.Red;

        public override MMRole Role => MMRole.Murderer;
        public override string SpawnMsg => $"<size=70>You are a {ColoredName}</size>";
        public override string SpawnInfoMsg => "<b><size=50>You must <color=#ff00ff>kill all</color> <color=#00ff00>innocents</color> and <color=#0000ff>detectives</color>.</size></b>";

        public string EquipmentMessage { get; } = "<color=#ff0000><size=30>You have recieved your equipment.</size></color>";

        public override MMRole[] RolesCanView { get; } = new MMRole[] { MMRole.Spectator, MMRole.Murderer };

        public void GiveEquipment(MMPlayer player)
        {
            player.RemoveInvalidItems();

            ItemBase weapon = player.Player.AddItem(ItemType.GunCOM18).Base;
            (weapon as Firearm).Status = new FirearmStatus(15, FirearmStatusFlags.MagazineInserted, 50);
            CustomItem.SerialItems.Add(weapon.ItemSerial, CustomItem.Items[MMItem.LockedEquipment]);
            CustomItem.SerialItems.Add(player.Player.AddItem(ItemType.SCP268).Serial, CustomItem.Items[MMItem.LockedEquipment]);
            CustomItem.SerialItems.Add(player.Player.AddItem(ItemType.Painkillers).Serial, CustomItem.Items[MMItem.UnprotectedItem]);
        }

        internal override void OnFirstSpawn(MMPlayer player)
        {
            base.OnFirstSpawn(player);
        }

        protected override void ChangingMMRole(MMPlayer player, CustomRole newRole)
        {
            if (MurderMystery.Singleton.GamemodeManager.Murderers939Vision)
            {
                player.Player.DisableAllEffects();
            }
        }
    }
}
