using UnityEngine;
using System.Reflection;
using HarmonyLib;
using ExtendedItemDataFramework;
using EpicLoot;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core.Configuration;
using Valheim.DropThat.Utilities;
using EpicLoot.LegendarySystem;
using Valheim.DropThat.Core;
using System.Collections.Generic;
using System.Linq;
using Valheim.DropThat.Caches;

namespace Valheim.DropThat.Drop.CharacterDropSystem.Modifiers.ModSpecific.ModEpicLoot
{
    internal class ModifierMagicItem : IDropModifier
    {
        private static ModifierMagicItem _instance;

        public static ModifierMagicItem Instance => _instance ??= new();

        private static MethodInfo InitializeMagicItem = AccessTools.Method(typeof(LootRoller), "InitializeMagicItem");

        public void Modify(DropContext context)
        {
            CharacterDropModConfigEpicLoot config;
            if (context.Extended.Config.TryGet(CharacterDropModConfigEpicLoot.ModName, out Config cfg) && cfg is CharacterDropModConfigEpicLoot modConfig)
            {
                config = modConfig;
            }
            else
            {
                return;
            }

#if DEBUG
            Log.LogDebug("Adding magic modifiers.");
#endif

            if (InitializeMagicItem is null)
            {
#if DEBUG
                Log.LogDebug("Couldn't find LootRoller.InitializeMagicItem.");
#endif
                return;
            }

            var itemDrop = ComponentCache.GetComponent<ItemDrop>(context.Item);

            if (EpicLoot.EpicLoot.CanBeMagicItem(itemDrop.m_itemData))
            {
                var extendedItemData = new ExtendedItemData(itemDrop.m_itemData);

                var rarity = RollRarity(config);

                if (rarity is Rarity.None)
                {
                    return;
                }

                if (rarity is Rarity.Unique)
                {
                    MakeUnique(itemDrop, extendedItemData, modConfig);
                }
                else
                {
                    //Make magic.
                    var epicLootRarity = RarityToItemRarity(rarity);
                    if (epicLootRarity is not null)
                    {
                        MakeMagic(epicLootRarity.Value, itemDrop, extendedItemData, context.Pos);
                    }
                }
            }
        }

        private void MakeUnique(ItemDrop itemDrop, ExtendedItemData itemData, CharacterDropModConfigEpicLoot config)
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

                    if (!legendaryInfo.Requirements.CheckRequirements(itemDrop.m_itemData, magicItem))
                    {
                        Log.LogWarning($"Attempted to roll Epic Loot unique legendary with id '{randomId}' for Drop That config entry '{config.SectionKey}' but requirements were not met. Skipping.");
                        return;
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
                    itemDrop.m_itemData = itemData;

                    InitializeMagicItem.Invoke(null, new[] { itemData });
                }
                else
                {
                    Log.LogWarning($"Attempted to roll Epic Loot unique legendary with id '{randomId}' but was unable to find matching info registered in Epic Loot.");
                }
            }
        }

        private void MakeMagic(ItemRarity rarity, ItemDrop itemDrop, ExtendedItemData itemData, Vector3 position)
        {
            MagicItemComponent magicComponent = itemData.AddComponent<MagicItemComponent>();

            var luck = LootRoller.GetLuckFactor(position);
            MagicItem magicItem = magicItem = LootRoller.RollMagicItem(rarity, itemData, luck);

            magicComponent.SetMagicItem(magicItem);
            itemDrop.m_itemData = itemData;

            InitializeMagicItem.Invoke(null, new[] { itemData });
        }

        private MagicItemEffectDefinition RollWeightedEffect(List<MagicItemEffectDefinition> magicEffects, bool removeSelected)
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

        private Rarity RollRarity(CharacterDropModConfigEpicLoot config)
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
            if (random <= ongoingSum)
                return Rarity.Unique;

            ongoingSum += config.RarityWeightLegendary;
            if (random <= ongoingSum)
                return Rarity.Legendary;

            ongoingSum += config.RarityWeightEpic;
            if (random <= ongoingSum)
                return Rarity.Epic;

            ongoingSum += config.RarityWeightRare;
            if (random <= ongoingSum)
                return Rarity.Rare;

            ongoingSum += config.RarityWeightMagic;
            if (random <= ongoingSum)
                return Rarity.Magic;

            return Rarity.None;
        }

        private ItemRarity? RarityToItemRarity(Rarity rarity)
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

    public enum Rarity
    {
        None,
        Magic,
        Rare,
        Epic,
        Legendary,
        Unique
    }
}
