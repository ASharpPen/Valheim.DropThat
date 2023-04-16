using ThatCore.Logging;
using UnityEngine;

namespace DropThat.Integrations.EpicLootIntegration;

internal static class ItemRoller
{
    internal static bool TryRollMagic(ItemDrop.ItemData itemData, Vector3 dropPos, ItemRollParameters parameters)
    {
        if (!EpicLoot.EpicLoot.CanBeMagicItem(itemData))
        {
            Log.DevelopmentOnly($"Item '{itemData.m_shared.m_name}' can't be made magic.");

            return false;
        }

        var rarity = ItemService.RollRarity(parameters);

        Log.DevelopmentOnly($"Item '{itemData.m_shared.m_name}' rolled rarity '{rarity}'.");

        if (rarity is Rarity.None)
        {
            return false;
        }

        if (rarity is Rarity.Unique)
        {
            return ItemService.TryMakeUnique(itemData, parameters);
        }
        else
        {
            //Make magic.
            var epicLootRarity = ItemService.RarityToItemRarity(rarity);
            if (epicLootRarity is not null)
            {
                ItemService.MakeMagic(
                    epicLootRarity.Value,
                    itemData,
                    dropPos);

                return true;
            }
        }

        return false;
    }
}
