using Exiled.API.Features.Items;
using System.Collections.Generic;

namespace MurderMystery.API.Features
{
    public static class CustomItemPool
    {
        public static HashSet<ushort> ProtectedItemIds { get; } = new HashSet<ushort>();

        internal static Item GiveAddToPool(ItemType itemType, MMPlayer player)
        {
            Item item = player.Player.AddItem(itemType);
            ProtectedItemIds.Add(item.Serial);
            return item;
        }
    }
}
