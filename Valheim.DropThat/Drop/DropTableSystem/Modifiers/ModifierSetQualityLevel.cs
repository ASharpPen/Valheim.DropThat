using UnityEngine;
using DropThat.Core;

namespace DropThat.Drop.DropTableSystem.Modifiers;

public class ModifierSetQualityLevel : IDropTableModifier
{
    private static ModifierSetQualityLevel _instance;

    public static ModifierSetQualityLevel Instance => _instance ??= new ModifierSetQualityLevel();

    public void Modify(DropModificationContext context)
    {
        int qualityLevel = context.Template?.Config?.SetQualityLevel ?? 0;

        if (qualityLevel <= 0)
        {
            return;
        }

        ItemDrop itemDrop = context.ItemDrop;

        if(itemDrop is null)
        {
            return;
        }

        Log.LogTrace($"Setting level of item '{context.Drop}' to {qualityLevel}");
        itemDrop.m_itemData.m_quality = qualityLevel;
    }

    public void Modify(ref ItemDrop.ItemData drop, DropTemplate template, Vector3 position)
    {
        int qualityLevel = template.Config?.SetQualityLevel ?? 0;

        if (qualityLevel <= 0)
        {
            return;
        }

        drop.m_quality = qualityLevel;
    }
}
