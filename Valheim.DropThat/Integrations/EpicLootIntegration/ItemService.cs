using EpicLoot;
using EpicLoot.LegendarySystem;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ThatCore.Logging;
using EpicLoot.Data;

namespace DropThat.Integrations.EpicLootIntegration;

internal static class ItemService
{
    public static bool TryMakeUnique(ItemDrop drop, ItemRollParameters parameters)
    {
        var itemData = drop.m_itemData;

        if (parameters.UniqueIds is null ||
            parameters.UniqueIds.Count == 0)
        {
            return false;
        }

        var randomId = parameters.UniqueIds[Random.Range(0, parameters.UniqueIds.Count)];

        if (!UniqueLegendaryHelper.TryGetLegendaryInfo(randomId, out LegendaryInfo legendaryInfo))
        {
            Log.Warning?.Log($"Attempted to roll Epic Loot unique legendary with id '{randomId}' but was unable to find matching info registered in Epic Loot.");
            return false;
        }

        MagicItem magicItem = new MagicItem
        {
            Rarity = ItemRarity.Legendary,
            LegendaryID = legendaryInfo.ID,
            DisplayName = legendaryInfo.Name,
        };

        if (!legendaryInfo.Requirements.CheckRequirements(itemData, magicItem))
        {
            Log.Warning?.Log($"Attempted to roll Epic Loot unique legendary with id '{randomId}' but requirements were not met. Skipping.");
            return false;
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
                    Log.Warning?.Log($"Unable to find a guaranteed Epic Loot magic effect '{effect.Type}' while rolling unique legendary with id '{randomId}'. Skipping effect.");
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

        MagicItemComponent magicComponent = itemData.Data().GetOrCreate<MagicItemComponent>();

        magicComponent.SetMagicItem(magicItem);

        drop.Save();

        LootRoller.InitializeMagicItem(itemData);

        return true;
    }

    public static void MakeMagic(ItemRarity rarity, ItemDrop drop, Vector3 position)
    {
        var itemData = drop.m_itemData;
        MagicItemComponent magicComponent = itemData.Data().GetOrCreate<MagicItemComponent>();

        var luck = LootRoller.GetLuckFactor(position);
        MagicItem magicItem = LootRoller.RollMagicItem(rarity, itemData, luck);

        Log.DevelopmentOnly("\t" + magicItem.Effects.Join(x => x.EffectType));

        magicComponent.SetMagicItem(magicItem);

        drop.Save();

        LootRoller.InitializeMagicItem(itemData);
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

    public static Rarity RollRarity(ItemRollParameters parameters)
    {
        var sumWeight =
            parameters.RarityWeightNone +
            parameters.RarityWeightMagic +
            parameters.RarityWeightRare +
            parameters.RarityWeightEpic +
            parameters.RarityWeightLegendary +
            parameters.RarityWeightUnique;

        var random = Random.Range(0, sumWeight);

        double ongoingSum = 0;

        ongoingSum += parameters.RarityWeightUnique;
        if (parameters.RarityWeightUnique > 0 && random <= ongoingSum)
            return Rarity.Unique;

        ongoingSum += parameters.RarityWeightLegendary;
        if (parameters.RarityWeightLegendary > 0 && random <= ongoingSum)
            return Rarity.Legendary;

        ongoingSum += parameters.RarityWeightEpic;
        if (parameters.RarityWeightEpic > 0 && random <= ongoingSum)
            return Rarity.Epic;

        ongoingSum += parameters.RarityWeightRare;
        if (parameters.RarityWeightRare > 0 && random <= ongoingSum)
            return Rarity.Rare;

        ongoingSum += parameters.RarityWeightMagic;
        if (parameters.RarityWeightMagic > 0 && random <= ongoingSum)
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
