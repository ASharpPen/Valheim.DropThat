using EpicLoot;
using EpicLoot.LegendarySystem;
using ExtendedItemDataFramework;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using DropThat.Configuration.ConfigTypes;
using DropThat.Core;
using DropThat.Utilities;

namespace DropThat.Integrations.EpicLootIntegration
{
    internal static class ItemService
    {
        private static MethodInfo InitializeMagicItem = AccessTools.Method(typeof(LootRoller), "InitializeMagicItem");

        public static ItemDrop.ItemData MakeUnique(ItemDrop.ItemData itemDrop, ExtendedItemData itemData, EpicLootItemConfiguration config)
        {
            var uniqueIds = config.UniqueIDs.Value.SplitByComma();

            if (uniqueIds.Count > 0)
            {
                var randomId = uniqueIds[Random.Range(0, uniqueIds.Count)];

                if (UniqueLegendaryHelper.TryGetLegendaryInfo(randomId, out LegendaryInfo legendaryInfo))
                {

                    MagicItem magicItem = new MagicItem
                    {
                        Rarity = ItemRarity.Legendary,
                        LegendaryID = legendaryInfo.ID,
                        DisplayName = legendaryInfo.Name,
                    };

                    if (!legendaryInfo.Requirements.CheckRequirements(itemDrop, magicItem))
                    {
                        Log.LogWarning($"Attempted to roll Epic Loot unique legendary with id '{randomId}' for Drop That config entry '{config.SectionKey}' but requirements were not met. Skipping.");
                        return null;
                    }

                    if (legendaryInfo.IsSetItem)
                    {
                        magicItem.SetID = UniqueLegendaryHelper.GetSetForLegendaryItem(legendaryInfo);
                    }

                    if ((legendaryInfo.GuaranteedMagicEffects?.Count ?? 0) > 0)
                    {
                        foreach (var effect in legendaryInfo.GuaranteedMagicEffects)
                        {
                            if (MagicItemEffectDefinitions.AllDefinitions.TryGetValue(effect.Type, out MagicItemEffectDefinition effectDefinition))
                            {
                                MagicItemEffect itemEffect = LootRoller.RollEffect(effectDefinition, ItemRarity.Legendary, effect.Values);
                                magicItem.Effects.Add(itemEffect);
                            }
                            else
                            {
                                Log.LogWarning($"Unable to find a guaranteed Epic Loot magic effect '{effect.Type}' while rolling unique legendary with id '{randomId}'. Skipping effect.");
                            }
                        }
                    }

                    var randomEffectCount = LootRoller.RollEffectCountPerRarity(ItemRarity.Legendary) - magicItem.Effects.Count;

                    if (randomEffectCount > 0)
                    {
                        List<MagicItemEffectDefinition> availableEffects = MagicItemEffectDefinitions.GetAvailableEffects(itemData, magicItem, -1);

                        for (int i = 0; i < randomEffectCount; ++i)
                        {
                            MagicItemEffectDefinition effectDefinition = RollWeightedEffect(availableEffects, false);
                            MagicItemEffect itemEffect = LootRoller.RollEffect(effectDefinition, ItemRarity.Legendary);
                            magicItem.Effects.Add(itemEffect);
                        }
                    }

                    MagicItemComponent magicComponent = itemData.AddComponent<MagicItemComponent>();

                    magicComponent.SetMagicItem(magicItem);

                    InitializeMagicItem.Invoke(null, new[] { itemData });

                    return itemData;
                }
                else
                {
                    Log.LogWarning($"Attempted to roll Epic Loot unique legendary with id '{randomId}' but was unable to find matching info registered in Epic Loot.");
                }
            }

            return null;
        }

        public static ItemDrop.ItemData MakeMagic(ItemRarity rarity, ItemDrop.ItemData itemDrop, ExtendedItemData itemData, Vector3 position)
        {
            MagicItemComponent magicComponent = itemData.AddComponent<MagicItemComponent>();

            var luck = LootRoller.GetLuckFactor(position);
            MagicItem magicItem = LootRoller.RollMagicItem(rarity, itemData, luck);

#if DEBUG
            Log.LogTrace("\t" + magicItem.Effects.Join(x => x.EffectType));
#endif

            magicComponent.SetMagicItem(magicItem);
            InitializeMagicItem.Invoke(null, new[] { itemData });

            return itemData;
        }

        private static MagicItemEffectDefinition RollWeightedEffect(List<MagicItemEffectDefinition> magicEffects, bool removeSelected)
        {
            var sumWeight = magicEffects.Sum(x => x.SelectionWeight);

            var random = Random.Range(0, sumWeight);

            float ongoingSum = 0;

            for (int i = 0; i < magicEffects.Count; ++i)
            {
                var magicEffect = magicEffects[i];

                ongoingSum += magicEffect.SelectionWeight;
                if (random <= ongoingSum)
                {
                    if (removeSelected)
                    {
                        magicEffects.RemoveAt(i);
                    }

                    return magicEffect;
                }
            }

            return magicEffects.Last();
        }

        public static Rarity RollRarity(EpicLootItemConfiguration config)
        {
            var sumWeight =
                config.RarityWeightNone +
                config.RarityWeightMagic +
                config.RarityWeightRare +
                config.RarityWeightEpic +
                config.RarityWeightLegendary +
                config.RarityWeightUnique;

            var random = Random.Range(0, sumWeight);

            float ongoingSum = 0;

            ongoingSum += config.RarityWeightUnique;
            if (config.RarityWeightUnique > 0 && random <= ongoingSum)
                return Rarity.Unique;

            ongoingSum += config.RarityWeightLegendary;
            if (config.RarityWeightLegendary > 0 && random <= ongoingSum)
                return Rarity.Legendary;

            ongoingSum += config.RarityWeightEpic;
            if (config.RarityWeightEpic > 0 && random <= ongoingSum)
                return Rarity.Epic;

            ongoingSum += config.RarityWeightRare;
            if (config.RarityWeightRare > 0 && random <= ongoingSum)
                return Rarity.Rare;

            ongoingSum += config.RarityWeightMagic;
            if (config.RarityWeightMagic > 0 && random <= ongoingSum)
                return Rarity.Magic;

            return Rarity.None;
        }

        public static ItemRarity? RarityToItemRarity(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.None => null,
                Rarity.Magic => ItemRarity.Magic,
                Rarity.Rare => ItemRarity.Rare,
                Rarity.Epic => ItemRarity.Epic,
                Rarity.Legendary => ItemRarity.Legendary,
                Rarity.Unique => ItemRarity.Legendary,
                _ => null,
            };
        }
    }
}
