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

        Log.Debug?.Log($"Setting durability of item '{itemDrop.name}' to {Durability}.");
        itemDrop.m_itemData.m_durability = Durability.Value;
    }

    public void Modify(ItemDrop.ItemData drop)
    {
        if (Durability is null ||
            Durability < 0)
        {
            return;
        }

        Log.Trace?.Log($"Setting durability of item '{drop.m_dropPrefab.name}' to {Durability}.");
        drop.m_durability = Durability.Value;
    }
}

internal static partial class IHaveItemModifierExtensions
{
    public static T ModifierDurability<T>(
        this T template,
        float? durability)
        where T : IHaveItemModifiers
    {
        if (durability is not null)
        {
            template.ItemModifiers.AddOrReplaceByType(new ModifierDurability(durability.Value));
        }
        else
        {
            template.ItemModifiers.RemoveAll(x => x is ModifierDurability);
        }

        return template;
    }
}