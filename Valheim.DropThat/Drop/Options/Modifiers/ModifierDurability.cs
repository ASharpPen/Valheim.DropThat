using DropThat.Caches;
using ThatCore.Extensions;
using ThatCore.Logging;
using UnityEngine;

namespace DropThat.Drop.Options.Modifiers;

public class ModifierDurability : IItemModifier
{
    public float? Durability { get; set; }

    public ModifierDurability() { }
    
    public ModifierDurability(float? durability)
    {
        Durability = durability;
    }

    public void Modify(GameObject drop)
    {
        if (Durability is null || 
            Durability < 0)
        {
            return;
        }

        var itemDrop = ComponentCache.Get<ItemDrop>(drop);

        if (itemDrop.IsNull())
        {
            return;
        }

        Log.LogDebug($"Setting durability of item '{itemDrop.name}' to {Durability}.");
        itemDrop.m_itemData.m_durability = Durability.Value;
    }
}
