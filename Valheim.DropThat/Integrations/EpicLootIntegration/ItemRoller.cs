using UnityEngine;
using ThatCore.Logging;

namespace DropThat.Integrations.EpicLootIntegration;

internal static class ItemRoller
{
    internal static bool TryRollMagic(ItemDrop drop, Vector3 dropPos, ItemRollParameters parameters)
    {
        var itemData = drop.m_itemData;

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
            return ItemService.TryMakeUnique(drop, parameters);
        }
        else
        {
            //Make magic.
            var epicLootRarity = ItemService.RarityToItemRarity(rarity);
            if (epicLootRarity is not null)
            {
                ItemService.MakeMagic(
                    epicLootRarity.Value,
                    drop,
                    dropPos);

                return true;
            }
        }

        return false;
    }
}
