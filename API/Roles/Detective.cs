using Exiled.API.Extensions;
using InventorySystem.Items.Firearms;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;
using MurderMystery.API.Interfaces;
using MurderMystery.Extensions;
using System.Collections.Generic;

namespace MurderMystery.API.Roles
{
    public sealed class Detective : CustomRole, IEquipment
    {
        public override string Name => "Detective";
        public override string ColoredName => "<color=#0000ff>Detective</color>";

        public override MMRole Role => MMRole.Detective;
        public override string SpawnMsg => $"<size=70>You are a {ColoredName}</size>";
        public override string SpawnInfoMsg => "<size=50>You must <color=#ff00ff>protect</color> <color=#00ff00>innocents</color> and <color=#ff00ff>kill</color> <color=#ff0000>murderers</color>.</size>";

        public string EquipmentMessage { get; } = "<color=#0000ff><size=30>You have recieved your equipment.</size></color>";

        public void GiveEquipment(MMPlayer player)
        {
            CustomItemPool.GiveAddToPool(ItemType.KeycardNTFCommander, player);
            (CustomItemPool.GiveAddToPool(ItemType.GunRevolver, player).Base as Firearm).Status = new FirearmStatus(6, FirearmStatusFlags.MagazineInserted, 553);
            player.Player.AddItem(ItemType.Medkit);
            player.Player.AddItem(ItemType.Painkillers);
        }

        public void RemoveEquipment(MMPlayer player) { }

        internal override void OnFirstSpawn(MMPlayer player)
        {
            base.OnFirstSpawn(player);

            List<MMPlayer> players = MMPlayer.List.GetRole(MMRole.Detective);

            for (int i = 0; i < players.Count; i++)
            {
                players[i].Player.SetPlayerInfoForTargetOnly(player.Player, Name);
            }
        }
    }
}
