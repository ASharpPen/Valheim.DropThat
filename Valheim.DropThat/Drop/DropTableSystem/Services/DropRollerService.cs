using System;
using System.Collections.Generic;
using System.Linq;
using DropThat.Caches;
using DropThat.Drop.DropTableSystem.Caches;
using DropThat.Drop.DropTableSystem.Models;
using DropThat.Drop.DropTableSystem.Wrapper;
using DropThat.Utilities;
using ThatCore.Logging;
using UnityEngine;

namespace DropThat.Drop.DropTableSystem.Services;

/// <summary>
/// Overhauled version of the vanilla DropTable drop rolling.
/// 
/// Support class for DropTableManager.
/// </summary>
internal static class DropRollerService
{
    public static List<GameObject> RollDrops(DropTable dropTable, GameObject source, List<DropTableDrop> drops)
    {
        var rolledDrops = RollDropsInternal(dropTable, source, drops);
        
        // Convert to GameObject.
        // In vanilla, these are the prefabs referenced by the ItemDrop.
        var convertedDrops = rolledDrops.SelectMany((drop) =>
        {
            GameObject dropObject = drop.DropData.m_item;

            if (drop.DropTemplate is not null)
            {
                // We need an object instance to track, so that we can run modifiers after instantiation.
                // So we create a Wrapper object that can be tracked if list and its order is modified later.
                // Multiple other mods are doing operations in here that muddy the tracking,
                // so this is necessary even if it is costly and fiddly.
                dropObject = dropObject.Wrap();
                DropTemplateCache.RegisterTemplate(dropObject, drop.DropTemplate);
            }

            // GameObject drops are handled individually.
            // Roll drop amount, and duplicate entries correspondingly.
            // TODO: Consider handling amount based on stack-size, and setting the size on the instantiated drop.
            int amount = UnityEngine.Random.Range(
                Math.Max(1, drop.DropData.m_stackMin),
                1 + drop.DropData.m_stackMax
                );

            var results = new GameObject[amount];

            for(int i = 0; i < amount; ++i)
            {
                results[i] = dropObject;
            }

            return results;
        });

        return convertedDrops
            .Where(x => x is not null)
            .ToList();
    }

    // TODO: Consider if we can change the overhaul to replace the initial list of drops instead, like Spawn That.
    // TODO: And then run modifications on AddItemToList, using the index for config lookup.
    public static List<ItemDrop.ItemData> RollItemDrops(DropTable dropTable, GameObject source, List<DropTableDrop> drops)
    {
        var rolledDrops = RollDropsInternal(dropTable, source, drops);

        if (Log.TraceEnabled)
        {
            Log.Trace?.Log($"Dropping {drops.Count} items:");
            foreach (var drop in drops)
            {
                Log.Trace?.Log($"\t{drop.DropData.m_item.name}");
            }
        }

        // Convert to ItemDrop.ItemData
        var convertedDrops = drops.Select((drop) =>
        {
            var itemDrop = ComponentCache.Get<ItemDrop>(drop.DropData.m_item);

            if (itemDrop is null)
            {
                return null;
            }

            ItemDrop.ItemData itemData = itemDrop.m_itemData.Clone();

            itemData.m_dropPrefab = drop.DropData.m_item;
            itemData.m_stack = UnityEngine.Random.Range(
                Math.Max(1, drop.DropData.m_stackMin),
                1 + Math.Min(itemData.m_shared.m_maxStackSize, drop.DropData.m_stackMax)
                );

            // Apply modifiers to ItemData.
            foreach (var modifier in drop.DropTemplate.ItemModifiers)
            {
                try
                {
                    modifier.Modify(itemData);
                }
                catch (Exception e)
                {
                    Log.Error?.Log(
                        $"Error while attempting to apply modifier '{modifier.GetType().Name}' " +
                        $"to '{drop.TableTemplate.PrefabName}.{drop.DropTemplate.Id}'.", e);
                }
            }

            return itemData;
        });

        return convertedDrops
            .Where(x => x is not null)
            .ToList();
    }

    public static List<DropTableDrop> RollDropsInternal(
        DropTable dropTable,
        GameObject source,
        List<DropTableDrop> drops)
    {
        bool skipDrop = UnityEngine.Random.value > dropTable.m_dropChance;

        if (skipDrop)
        {
            Log.Trace?.Log($"DropTable chance rolled too high. Nothing will be dropped for '{source.GetCleanedName()}'.");

            return new(0);
        }

        // Gather drops and configs
        var possibleDrops = GetPossibleDrops(drops, source);

        if (Log.TraceEnabled)
        {
            Log.Trace?.Log($"Possible drops: ");
            foreach (var drop in drops)
            {
                int id = drop.DropTemplate is not null
                    ? drop.DropTemplate.Id
                    : drop.DropTableIndex;

                Log.Trace?.Log($"\t{id}: {drop.DropData.m_item.name}");
            }
        }

        float sumWeight = possibleDrops.Sum(x => x.DropData.m_weight);

        int dropCount = UnityEngine.Random.Range(
            dropTable.m_dropMin,
            dropTable.m_dropMax + 1);

        List<DropTableDrop> results = new(dropCount);

        for(int i = 0; i < dropCount; i++)
        {
            float rolledWeight = UnityEngine.Random.Range(0, sumWeight);
            float accumulatedWeight = 0;

            // Scan through drops, accumulating weight until we hit the rolledWeight.
            for (int j = 0; j < possibleDrops.Count; ++j)
            {
                var drop = possibleDrops[j];
                accumulatedWeight += drop.DropData.m_weight;

                if (rolledWeight <= accumulatedWeight)
                {
                    // Found the drop
                    results.Add(drop);

                    // If set to drop only once, remove from list of possible drops.
                    if (dropTable.m_oneOfEach)
                    {
                        possibleDrops.RemoveAt(j);
                        sumWeight -= drop.DropData.m_weight;
                    }

                    break;
                }
            }
        }

        return results;
    }

    private static List<DropTableDrop> GetPossibleDrops(List<DropTableDrop> drops, GameObject source)
    {
        List<DropTableDrop> workingList = new();

        for (int i = 0; i < drops.Count; ++i)
        {
            var drop = drops[i];

            if (drop.DropTemplate is not null)
            {
                // Found config for drop.

                if (drop.DropTemplate.Enabled == false)
                {
                    Log.Trace?.Log(
                        $"Filtering drop '{drop.TableTemplate.PrefabName}.{drop.DropTemplate.Id}' " +
                        $"due to not being enabled.");

                    continue;
                }

                var dropContext = new DropContext(source, drop.TableData, drop.DropData)
                {
                    DropConfig = drop.DropTemplate,
                    DropTableConfig = drop.TableTemplate
                };

                var validDrop = drop.DropTemplate.Conditions.All(cond =>
                {
                    try
                    {
                        var status = cond.IsValid(dropContext);

                        Log.Trace?.Log(
                            $"Filtering drop '{drop.TableTemplate.PrefabName}.{drop.DropTemplate.Id}' " +
                            $"due to invalid condition '{cond?.GetType()?.Name}'.");

                        return status;
                    }
                    catch (Exception e)
                    {
                        Log.Error?.Log(
                            $"Error while attempting to check condition '{cond?.GetType()?.Name}' " +
                            $"for config '{drop.TableTemplate.PrefabName}.{drop.DropTemplate.Id}'. " +
                            $"Condition will be ignored", e);
                        return true;
                    }
                });

                if (validDrop)
                {
                    workingList.Add(new()
                    {
                        CurrentIndex = i,
                        DropTableIndex = drop.DropTableIndex,
                        TableData = drop.TableData,
                        DropData = drop.DropData,
                        TableTemplate = drop.TableTemplate,
                        DropTemplate = drop.DropTemplate,
                    });
                }
            }
            else
            {
                // No config for drop. Just add it normally.

                workingList.Add(new()
                {
                    CurrentIndex = i,
                    DropTableIndex = drop.DropTableIndex,
                    TableData = drop.TableData,
                    DropData = drop.DropData
                });
            }
        }

        return workingList;
    }
}
