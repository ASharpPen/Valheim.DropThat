using System;
using System.Collections.Generic;
using DropThat.Caches;
using DropThat.Drop.DropTableSystem.Models;
using DropThat.Drop.DropTableSystem.Wrapper;
using DropThat.Drop.Options;
using ThatCore.Extensions;
using ThatCore.Logging;
using UnityEngine;
using static ItemDrop;

namespace DropThat.Drop.DropTableSystem.Services;

internal static class DropScalerService
{
    /// <summary>
    /// GameObject drops are handled individually.
    /// Roll drop amount, and duplicate entries correspondingly.
    /// TODO: Consider handling amount based on stack-size, and setting the size on the instantiated drop.
    /// <returns></returns>
    public static GameObject[] ScaleDropsAsGameObjects(DropTableDrop drop)
    {
        int amount = CalculateAmount(drop);

        GameObject[] results = new GameObject[amount];

        for (int i = 0; i < amount; i++)
        {
            GameObject dropObject = drop.DropData.m_item;

            if (drop.DropTemplate is not null)
            {
                // We need an object instance to track, so that we can run modifiers after instantiation.
                // So we create a Wrapper object that can be tracked if list and its order is modified later.
                // Multiple other mods are doing operations in here that muddy the tracking,
                // so this is necessary even if it is costly and fiddly.
                var wrapper = dropObject.Wrap();

                // Store reference to template objects
                wrapper.Drop = drop;

                // Use wrapper object instead of original prefab.
                dropObject = wrapper.gameObject;
            }

            results[i] = dropObject;
        }

        return results;
    }

    // Just a small cache for when returning single items.
    private readonly static ItemDrop.ItemData[] Item = new ItemDrop.ItemData[1];

    public static ICollection<ItemDrop.ItemData> ScaleDropsAsItemData(GameObject source, DropTableDrop drop)
    {
        var itemDrop = ComponentCache.Get<ItemDrop>(drop.DropData.m_item);

        if (itemDrop.IsNull())
        {
            return Array.Empty<ItemDrop.ItemData>();
        }

        // Calculate resource scaled and roll min-max amount
        var amount = CalculateAmount(drop);

        int maxStackSize = itemDrop.m_itemData.m_shared.m_maxStackSize;

        // Split into multiple items if stackSize less than amount.
        if (amount > maxStackSize)
        {
            int stacks = Mathf.CeilToInt(amount / (float)maxStackSize);

#if DEBUG || TEST
            Log.Debug?.Log($"Splitting '{amount}' drops of '{itemDrop.m_itemData.m_shared.m_name}' into '{stacks}' stacks.");
#endif
            List<ItemDrop.ItemData> drops = new(stacks);

            int remainingAmount = amount;

            for(int i = 0; i < stacks; ++i)
            {
                var dropAmount = Math.Min(remainingAmount, maxStackSize);

                ItemDrop.ItemData dropItemData = CreateItemDataDrop(source, drop, itemDrop, dropAmount);
#if DEBUG
                Log.Debug?.Log($"    {dropItemData.m_shared.m_name}: {dropAmount}");
#endif

                drops.Add(dropItemData);
                remainingAmount -= dropAmount;
            }

            return drops;
        }
        else
        {
            // Drop a single item
            Item[0] = CreateItemDataDrop(source, drop, itemDrop, amount);
            return Item;
        }
    }

    private static ItemData CreateItemDataDrop(GameObject source, DropTableDrop drop, ItemDrop itemDrop, int amount)
    {
        ItemDrop.ItemData dropItemData = itemDrop.m_itemData.Clone();

        dropItemData.m_dropPrefab = drop.DropData.m_item;
        dropItemData.m_stack = amount;

        if (drop.DropTemplate is not null)
        {
            // Apply modifiers to ItemData.
            ItemModifierContext<ItemDrop.ItemData> dropContext = new()
            {
                Item = dropItemData,
                Position = source.transform.position,
            };

            foreach (var modifier in drop.DropTemplate.ItemModifiers)
            {
                try
                {
                    modifier.Modify(dropContext);
                }
                catch (Exception e)
                {
                    Log.Error?.Log(
                        $"Error while attempting to apply modifier '{modifier.GetType().Name}' " +
                        $"to '{drop.TableTemplate.PrefabName}.{drop.DropTemplate.Id}'.", e);
                }
            }
        }

        return dropItemData;
    }

    /// <summary>
    /// Calculate scaled amount.
    /// Based on rolling the configured min-max,
    /// and adding world-modifier resource scaling if enabled.
    /// </summary>
    /// <param name="drop"></param>
    /// <returns></returns>
    public static int CalculateAmount(DropTableDrop drop)
    {
        int minAmount = Math.Max(1, drop.DropData.m_stackMin);
        int maxAmount = drop.DropData.m_stackMax + 1;

        int preScaledAmount = UnityEngine.Random.Range(minAmount, maxAmount);

        if (drop.DropData.m_dontScale)
        {
            return preScaledAmount;
        }

        float scaledAmount = preScaledAmount * Game.m_resourceRate;

        float remainderAmount = scaledAmount % 1f;

        int finalAmount = (int)scaledAmount;

        const float epsilon = 0.001f;
        if (remainderAmount > epsilon &&
            remainderAmount <= UnityEngine.Random.Range(0, 1f))
        {
            finalAmount++;
        }

#if DEBUG
        Log.Debug?.Log($"Calculated scaled amount '{scaledAmount}' resulting in '{(int)scaledAmount}' + '1 * {remainderAmount * 100}%'");
#endif
        return finalAmount;
    }
}
