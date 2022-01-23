using Exiled.Events.EventArgs;
using MurderMystery.API.Enums;
using MurderMystery.API.Features;

namespace MurderMystery.API.Items
{
    public class UnprotectedItem : CustomItem
    {
        public override MMItem Item => MMItem.UnprotectedItem;

        public override bool IsEquipmentItem => false;

        public override void PickingUpItem(PickingUpItemEventArgs ev)
        {
            ev.IsAllowed = true;
        }

        public override void DroppingItem(DroppingItemEventArgs ev)
        {
            ev.IsAllowed = true;
        }
    }
}
