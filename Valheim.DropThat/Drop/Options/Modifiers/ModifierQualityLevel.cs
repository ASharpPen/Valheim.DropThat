using DropThat.Caches;
using ThatCore.Extensions;
using ThatCore.Logging;
using UnityEngine;

namespace DropThat.Drop.Options.Modifiers;

public sealed class ModifierQualityLevel : IItemModifier
{
    public int? QualityLevel { get; set; }

    public bool IsPointless() =>
        QualityLevel is null ||
        QualityLevel <= 0;

    public void Modify(ItemModifierContext<GameObject> drop)
    {
        if (IsPointless())
        {
            return;
        }

        var itemDrop = ComponentCache.Get<ItemDrop>(drop.Item);

        if (itemDrop.IsNull())
        {
            return;
        }

        Log.Trace?.Log($"Setting quality level of item '{itemDrop.name}' to {QualityLevel}.");
        itemDrop.m_itemData.m_quality = QualityLevel.Value;
    }

    public void Modify(ItemModifierContext<ItemDrop.ItemData> drop)
    {
        if (IsPointless())
        {
            return;
        }

        Log.Trace?.Log($"Setting quality level of item '{drop.Item.m_dropPrefab.name}' to {QualityLevel}.");
        drop.Item.m_quality = QualityLevel.Value;
    }
}