using UnityEngine;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Integrations.EpicLootIntegration;

internal static class ItemRoller
{
    internal static bool TryRollMagic(ItemDrop.ItemData itemData, Vector3 dropPos, ItemRollParameters parameters)
    {
        if (!EpicLoot.EpicLoot.CanBeMagicItem(itemData))
        {
#if DEBUG
            Log.LogTrace($"Item '{itemData.m_shared.m_name}' can't be made magic.");
#endif
            return false;
        }

        var rarity = ItemService.RollRarity(parameters);

#if DEBUG
        Log.LogTrace($"Item '{itemData.m_shared.m_name}' rolled rarity '{rarity}'.");
#endif

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
