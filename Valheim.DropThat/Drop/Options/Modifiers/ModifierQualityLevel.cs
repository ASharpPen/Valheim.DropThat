﻿using DropThat.Caches;
using ThatCore.Extensions;
using ThatCore.Logging;
using UnityEngine;

namespace DropThat.Drop.Options.Modifiers;

public sealed class ModifierQualityLevel : IItemModifier
{
    public int? QualityLevel { get; set; }

    public void Modify(ItemModifierContext<GameObject> drop)
    {
        if (QualityLevel is null ||
            QualityLevel < 0)
        {
            return;
        }

        var itemDrop = ComponentCache.Get<ItemDrop>(drop.Item);

        if (itemDrop.IsNull())
        {
            return;
        }

        Log.Trace?.Log($"Setting quality level of item '{itemDrop.name}' to {QualityLevel}.");
        itemDrop.m_itemData.m_durability = QualityLevel.Value;
    }

    public void Modify(ItemModifierContext<ItemDrop.ItemData> drop)
    {
        if (QualityLevel is null ||
            QualityLevel < 0)
        {
            return;
        }

        Log.Trace?.Log($"Setting quality level of item '{drop.Item.m_dropPrefab.name}' to {QualityLevel}.");
        drop.Item.m_quality = QualityLevel.Value;
    }
}

internal static partial class IHaveItemModifierExtensions
{
    public static T ModifierQualityLevel<T>(
        this T template,
        int? qualityLevel)
        where T : IHaveItemModifiers
    {
        if (qualityLevel >= 0)
        {
            template.ItemModifiers
                .GetOrCreate<ModifierQualityLevel>()
                .QualityLevel = qualityLevel.Value;
        }
        else
        {
            template.ItemModifiers.Remove<ModifierQualityLevel>();
        }

        return template;
    }
}