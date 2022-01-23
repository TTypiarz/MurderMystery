using Exiled.Events.EventArgs;
using MEC;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;

namespace MurderMystery.API.Items
{
    public class DetectiveWeapon : CustomItem
    {
        public override MMItem Item => MMItem.DetectiveWeapon;

        public override bool IsEquipmentItem => true;

        public override void PickingUpItem(PickingUpItemEventArgs ev)
        {
            if (MMPlayer.Get(ev.Player, out MMPlayer player))
            {
                if (player.Role == MMRole.Innocent && ev.Pickup.Type == ItemType.GunRevolver)
                {
                    ev.IsAllowed = true;
                    player.Role = MMRole.Detective;
                    return;
                }
            }

            ev.IsAllowed = false;
        }

        public override void DroppingItem(DroppingItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
