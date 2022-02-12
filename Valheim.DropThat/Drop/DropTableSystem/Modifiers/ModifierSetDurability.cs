using UnityEngine;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Drop.DropTableSystem.Modifiers;

public class ModifierSetDurability : IDropTableModifier
{
    private static ModifierSetDurability _instance;

    public static ModifierSetDurability Instance => _instance ??= new();

    public void Modify(DropModificationContext context)
    {
        float durability = context.Template?.Config?.SetDurability ?? -1f;

        if (durability < 0)
        {
            return;
        }

        ItemDrop itemDrop = context.ItemDrop;

        if (itemDrop is null)
        {
            return;
        }

        Log.LogTrace($"Setting durability of item '{context.Drop}' to {durability}");
        itemDrop.m_itemData.m_durability = durability;
    }

    public void Modify(ref ItemDrop.ItemData drop, DropTemplate template, Vector3 position)
    {
        float durability = template.Config?.SetDurability ?? -1f;

        if (durability < 0)
        {
            return;
        }

        drop.m_durability = durability;
    }
}
