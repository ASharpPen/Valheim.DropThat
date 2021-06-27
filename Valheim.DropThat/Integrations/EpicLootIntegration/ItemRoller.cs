using ExtendedItemDataFramework;
using Valheim.DropThat.Configuration.ConfigTypes;
using UnityEngine;

namespace Valheim.DropThat.Integrations.EpicLootIntegration
{
    internal static class ItemRoller
    {
        public static ItemDrop.ItemData Roll(ItemDrop.ItemData itemData, Vector3 dropPos, EpicLootItemConfiguration config)
        {
            if (!EpicLoot.EpicLoot.CanBeMagicItem(itemData))
            {
                return null;
            }

            var extendedItemData = new ExtendedItemData(itemData);

            var rarity = ItemService.RollRarity(config);

            if (rarity is Rarity.None)
            {
                return null;
            }

            ItemDrop.ItemData magicItemData = null;

            if (rarity is Rarity.Unique)
            {
                magicItemData = ItemService.MakeUnique(itemData, extendedItemData, config);
            }
            else
            {
                //Make magic.
                var epicLootRarity = ItemService.RarityToItemRarity(rarity);
                if (epicLootRarity is not null)
                {
                    magicItemData = ItemService.MakeMagic(
                        epicLootRarity.Value,
                        itemData,
                        extendedItemData,
                        dropPos);
                }
            }

            return magicItemData;
        }
    }
}
