using DropThat.Integrations.EpicLootIntegration;
using UnityEngine;
using System.Collections.Generic;
using ThatCore.Extensions;
using DropThat.Caches;
using System.Linq;
using DropThat.Integrations;
using ThatCore.Models;

namespace DropThat.Drop.Options.Modifiers.ModEpicLoot;

public class ModifierEpicLootItem : IItemModifier
{
    public float? RarityWeightNone { get; set; }
    public float? RarityWeightMagic { get; set; }
    public float? RarityWeightRare { get; set; }
    public float? RarityWeightEpic { get; set; }
    public float? RarityWeightLegendary { get; set; }
    public float? RarityWeightUnique { get; set; }
    public List<string> UniqueIds { get; set; }

    public ModifierEpicLootItem() { }

    public ModifierEpicLootItem(
        float? rarityWeightNone,
        float? rarityWeightMagic,
        float? rarityWeightRare,
        float? rarityWeightEpic,
        float? rarityWeightLegendary,
        float? rarityWeightUnique,
        IEnumerable<string> uniqueIds)
    {
        RarityWeightNone = rarityWeightNone;
        RarityWeightMagic = rarityWeightMagic;
        RarityWeightRare = rarityWeightRare;
        RarityWeightEpic = rarityWeightEpic;
        RarityWeightLegendary = rarityWeightLegendary;
        RarityWeightUnique = rarityWeightUnique;
        uniqueIds = uniqueIds.ToList();
    }

    public bool IsPointless =>
        RarityWeightNone +
        RarityWeightMagic +
        RarityWeightRare +
        RarityWeightEpic +
        RarityWeightLegendary == 0 &&
        (UniqueIds?.Count ?? 0) == 0
        ;

    public void Modify(ItemModifierContext<GameObject> drop)
    {
        if (drop.Item.IsNull() ||
            IsPointless)
        {
            return;
        }

        var itemDrop = ComponentCache.Get<ItemDrop>(drop.Item);

        if (ItemRoller.TryRollMagic(
            itemDrop.m_itemData,
            itemDrop.transform.position,
            new ItemRollParameters
            {
                RarityWeightNone = RarityWeightNone ?? 0,
                RarityWeightMagic = RarityWeightMagic ?? 0,
                RarityWeightRare = RarityWeightRare ?? 0,
                RarityWeightEpic = RarityWeightEpic ?? 0,
                RarityWeightLegendary = RarityWeightLegendary ?? 0,
                RarityWeightUnique = RarityWeightUnique ?? 0,
                UniqueIds = UniqueIds,
            }))
        {
            itemDrop.Save();
        }
    }

    public void Modify(ItemModifierContext<ItemDrop.ItemData> drop)
    {
        if (drop?.Item is null ||
            IsPointless)
        {
            return;
        }

        ItemRoller.TryRollMagic(
            drop.Item,
            drop.Position,
            new ItemRollParameters
            {
                RarityWeightNone = RarityWeightNone ?? 0,
                RarityWeightMagic = RarityWeightMagic ?? 0,
                RarityWeightRare = RarityWeightRare ?? 0,
                RarityWeightEpic = RarityWeightEpic ?? 0,
                RarityWeightLegendary = RarityWeightLegendary ?? 0,
                RarityWeightUnique = RarityWeightUnique ?? 0,
                UniqueIds = UniqueIds,
            })
            ;
    }
}

internal static partial class IHaveItemModifierExtensions
{
    public static IHaveItemModifiers ModifierEpicLootItem(
        this IHaveItemModifiers template,
        Optional<float?> rarityWeightNone,
        Optional<float?> rarityWeightMagic,
        Optional<float?> rarityWeightRare,
        Optional<float?> rarityWeightEpic,
        Optional<float?> rarityWeightLegendary,
        Optional<float?> rarityWeightUnique,
        Optional<List<string>> uniqueIds
        )
    {
        var modifier = template.ItemModifiers
            .OfType<ModifierEpicLootItem>()
            .FirstOrDefault();

        if (modifier is null)
        {
            modifier = new();
            template.ItemModifiers.Add(modifier);
        }

        rarityWeightNone.DoIfSet((x) => modifier.RarityWeightNone = x);
        rarityWeightMagic.DoIfSet((x) => modifier.RarityWeightMagic = x);
        rarityWeightRare.DoIfSet((x) => modifier.RarityWeightRare = x);
        rarityWeightEpic.DoIfSet((x) => modifier.RarityWeightEpic = x);
        rarityWeightLegendary.DoIfSet((x) => modifier.RarityWeightLegendary = x);
        rarityWeightUnique.DoIfSet((x) => modifier.RarityWeightUnique = x);
        uniqueIds.DoIfSet((x) => modifier.UniqueIds = x);

        return template;
    }

    public static ModifierEpicLootItem ModifierEpicLootItem<T>(
        this T template)
        where T : IHaveItemModifiers
    {
        if (!InstallationManager.EpicLootInstalled)
        {
            return null;
        }

        var modifier = template
            .ItemModifiers
            .OfType<ModifierEpicLootItem>()
            .FirstOrDefault();

        if (modifier is null)
        {
            modifier = new();
            template.ItemModifiers.Add(modifier);
        }

        return modifier;
    }
}