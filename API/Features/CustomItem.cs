using Exiled.Events.EventArgs;
using MurderMystery.API.Enums;
using MurderMystery.API.Items;
using System.Collections.Generic;

namespace MurderMystery.API.Features
{
    public abstract class CustomItem
    {
        public static Dictionary<MMItem, CustomItem> Items { get; } = new Dictionary<MMItem, CustomItem>()
        {
            [MMItem.UnprotectedItem] = new UnprotectedItem(),
            [MMItem.DetectiveWeapon] = new DetectiveWeapon(),
            [MMItem.LockedEquipment] = new LockedEquipment()
        };

        public static Dictionary<ushort, CustomItem> SerialItems { get; } = new Dictionary<ushort, CustomItem>();

        public abstract MMItem Item { get; }
        public abstract bool IsEquipmentItem { get; }

        public virtual void PickingUpItem(PickingUpItemEventArgs ev) { }
        public virtual void DroppingItem(DroppingItemEventArgs ev) { }
    }
}
