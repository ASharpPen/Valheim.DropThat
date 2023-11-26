using DropThat.Integrations.EpicLootIntegration;
using UnityEngine;
using System.Collections.Generic;
using ThatCore.Extensions;
using DropThat.Caches;
using System.Linq;

namespace DropThat.Drop.Options.Modifiers.ModEpicLoot;

public sealed class ModifierEpicLootItem : IItemModifier
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
        UniqueIds = uniqueIds.ToList();
    }

    public bool IsPointless =>
        RarityWeightNone +
        RarityWeightMagic +
        RarityWeightRare +
        RarityWeightEpic +
        RarityWeightLegendary +
        RarityWeightUnique == 0 &&
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