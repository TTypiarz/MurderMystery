using Exiled.Events.EventArgs;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;

namespace MurderMystery.API.Items
{
    public class LockedEquipment : CustomItem
    {
        public override MMItem Item => MMItem.LockedEquipment;

        public override bool IsEquipmentItem => true;

        public override void PickingUpItem(PickingUpItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public override void DroppingItem(DroppingItemEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}
