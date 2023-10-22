using System;
using System.Collections.Generic;
using System.Linq;
using EpicLoot;
using UnityEngine;
using Valheim.DropThat.Core;
using Valheim.DropThat.Drop.DropTableSystem.Caches;
using Valheim.DropThat.Drop.DropTableSystem.Managers;
using Valheim.DropThat.Drop.DropTableSystem.Wrapper;
using static ItemDrop;

namespace Valheim.DropThat.Drop.DropTableSystem
{
    internal static class DropTableManager
    {
        public static List<ItemDrop.ItemData> GetItemDrops(DropTable dropTable, DropSourceTemplateLink context)
        {
            var drops = GetDrops(
                dropTable,
                context,
                ConvertTemplateToItem);

#if DEBUG
            Log.LogDebug($"Dropping {drops.Count} items:");
            foreach (var drop in drops)
            {
                Log.LogDebug($"\t{drop.m_shared.m_name}");
            }
#endif

            return drops;
        }

        public static List<GameObject> GetDrops(DropTable dropTable, DropSourceTemplateLink context)
        {
            var drops = GetDrops(
                dropTable,
                context,
                ConvertTemplateToDrop);

#if DEBUG
            Log.LogDebug($"Dropping {drops.Count} items:");
            foreach (var drop in drops)
            {
                Log.LogDebug($"\t{drop}");
            }
#endif
            return drops;
        }

        private static List<T> GetDrops<T>(
            DropTable dropTable,
            DropSourceTemplateLink context,
            Func<DropTemplate, IEnumerable<T>> DropConverter) 
            where T : class
        {
            var dropTemplates = DropConfigManager.GetPossibleDrops(context, dropTable);

            if (dropTemplates.Count == 0)
            {
#if DEBUG
                Log.LogDebug("Skipping drop due to no templates.");
#endif
                return new(0);
            }

            var entityConfig = context.EntityConfig;

            bool skipDrop = entityConfig is null
                ? UnityEngine.Random.value > dropTable.m_dropChance
                : UnityEngine.Random.value > (entityConfig.SetDropChance / 100);

            if (skipDrop)
            {
#if DEBUG
                Log.LogDebug("Skipping drop due to table chance.");
#endif
                return new(0);
            }

            var workingList = new List<DropTemplate>(dropTemplates);

            workingList = DropConditionManager.Filter(context, workingList);

#if DEBUG
            Log.LogDebug("Drops after filter:");
            foreach (var drop in workingList)
            {
                Log.LogDebug("\t" + drop.Drop.m_item.name);
            }
#endif

            float sumWeight = workingList.Sum(x => x.Drop.m_weight);

            int dropCount = entityConfig is null
                ? UnityEngine.Random.Range(dropTable.m_dropMin, dropTable.m_dropMax + 1)
                : UnityEngine.Random.Range(entityConfig.SetDropMin, entityConfig.SetDropMax + 1);

            bool dropOnlyOnce = entityConfig is null
                ? dropTable.m_oneOfEach
                : entityConfig.SetDropOnlyOnce;

            List<T> result = new(dropCount);

#if DEBUG
            Log.LogDebug($"Rolling {dropCount} drops.");
#endif


            for (int i = 0; i < dropCount; ++i)
            {
                float weight = UnityEngine.Random.Range(0, sumWeight);
                float accumulatedWeight = 0;

                try
                {
                    bool addedDrop = false;

                    // Select drop based on weight.
                    foreach (var template in workingList)
                    {
                        accumulatedWeight += template.Drop.m_weight;

                        if (weight <= accumulatedWeight)
                        {
                            result.AddRange(DropConverter(template));

                            if (dropOnlyOnce)
                            {
                                workingList.Remove(template);
                                sumWeight -= template.Drop.m_weight;
                            }

                            addedDrop = true;
                            break;
                        }
                    }

                    // Seems like a sanity check. If IG did their logic properly, it should never be necessary. Consider just removing this piece of code.
                    if (!addedDrop && workingList.Count > 0)
                    {
                        result.AddRange(DropConverter(workingList.First()));
                    }
                }
                catch (Exception e)
                {
                    Log.LogWarning("Error while rolling drop. Skipping roll\n", e);
                }
            }

            return result;
        }

        private readonly static ItemDrop.ItemData[] Item = new ItemDrop.ItemData[1];

        private static IEnumerable<ItemDrop.ItemData> ConvertTemplateToItem(DropTemplate template)
        {
            try
            {
                ItemDrop.ItemData itemData = template.Drop.m_item.GetComponent<ItemDrop>().m_itemData.Clone();

                // Calculate resource scaled amount
                var amount = CalculateAmount(template);

                // Split into multiple items if stackSize less than amount.
                if (amount > itemData.m_shared.m_maxStackSize)
                {
                    int stacks = Mathf.CeilToInt(amount / (float)itemData.m_shared.m_maxStackSize);

#if DEBUG
                    Log.LogDebug($"Splitting '{amount}' drops of '{itemData.m_shared.m_name}' into '{stacks}' stacks.");
#endif

                    List<ItemDrop.ItemData> drops = new(stacks);

                    int remainingAmount = amount;

                    for (int i = 0; i < stacks; ++i)
                    {
                        ItemDrop.ItemData dropItemData = template.Drop.m_item.GetComponent<ItemDrop>().m_itemData.Clone();

                        var dropAmount = Math.Min(remainingAmount, itemData.m_shared.m_maxStackSize);

#if DEBUG
                        Log.LogDebug($"    {itemData.m_shared.m_name}: {dropAmount}");
#endif


                        dropItemData.m_dropPrefab = template.Drop.m_item.Wrap();
                        dropItemData.m_stack = dropAmount;
                        dropItemData.m_quality = template.Config?.SetQualityLevel ?? 1;
                        dropItemData.m_durability = (template.Config?.SetDurability ?? -1f) >= 0
                            ? template.Config.SetDurability
                            : dropItemData.m_durability; //Use whatever is default

                        // Store reference to both wrapped prefab and ItemData object, to ensure we can keep track of it.
                        DropTemplateCache.RegisterTemplate(dropItemData, template);
                        DropTemplateCache.RegisterTemplate(dropItemData.m_dropPrefab, template);

                        drops.Add(dropItemData);
                        remainingAmount -= dropAmount;
                    }

                    return drops;
                }
                else
                {
                    itemData.m_dropPrefab = template.Drop.m_item.Wrap();
                    itemData.m_stack = amount;
                    itemData.m_quality = template.Config?.SetQualityLevel ?? 1;
                    itemData.m_durability = (template.Config?.SetDurability ?? -1f) >= 0
                        ? template.Config.SetDurability
                        : itemData.m_durability; //Use whatever is default

                    // Store reference to both wrapped prefab and ItemData object, to ensure we can keep track of it.
                    DropTemplateCache.RegisterTemplate(itemData, template);
                    DropTemplateCache.RegisterTemplate(itemData.m_dropPrefab, template);

                    Item[0] = itemData;
                    return Item;
                }
            }
            catch (Exception e)
            {
                Log.LogError("Error while attempting to prepare new item data", e);
                return Enumerable.Empty<ItemDrop.ItemData>();
            }
        }

        private static IEnumerable<GameObject> ConvertTemplateToDrop(DropTemplate template)
        {
            var drop = template.Drop.m_item.Wrap();
            DropTemplateCache.RegisterTemplate(drop, template);

            var amount = CalculateAmount(template);

            GameObject[] result = new GameObject[amount];

            for (int i = 0; i < amount; ++i)
            {
                result[i] = drop;
            }

            return result;
        }

        private static int CalculateAmount(DropTemplate template)
        {
            int minAmount = Math.Max(1, template.Drop.m_stackMin);
            int maxAmount = template.Drop.m_stackMax + 1;

            int preScaledAmount = UnityEngine.Random.Range(minAmount, maxAmount);

#if DEBUG
            Log.LogDebug($"Calculated prescaled amount '{preScaledAmount}' in range [{minAmount}, {maxAmount}]");
#endif

            if (template.Drop.m_dontScale)
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
            Log.LogDebug($"Calculated scaled amount '{scaledAmount}' resulting in '{(int)scaledAmount}' + '1 * {remainderAmount*100}%'");
#endif
            return finalAmount;
        }
    }
}
