using DropThat.Caches;
using ThatCore.Extensions;
using ThatCore.Logging;
using UnityEngine;

namespace DropThat.Drop.Options.Modifiers;

public sealed class ModifierDurability : IItemModifier
{
    public float? Durability { get; set; }

    public bool IsPointless() =>
        Durability is null ||
        Durability < 0;

    public void Modify(ItemModifierContext<GameObject> drop)
    {
        if (Durability is null || 
            Durability < 0)
        {
            return;
        }

        var itemDrop = ComponentCache.Get<ItemDrop>(drop.Item);

        if (itemDrop.IsNull())
        {
            return;
        }

        Log.Trace?.Log($"Setting durability of item '{itemDrop.name}' to {Durability}.");
        itemDrop.m_itemData.m_durability = Durability.Value;
    }

    public void Modify(ItemModifierContext<ItemDrop.ItemData> drop)
    {
        if (Durability is null ||
            Durability < 0)
        {
            return;
        }

        Log.Trace?.Log($"Setting durability of item '{drop.Item.m_dropPrefab.name}' to {Durability}.");
        drop.Item.m_durability = Durability.Value;
    }
}