using DropThat.Caches;
using ThatCore.Extensions;
using ThatCore.Logging;
using UnityEngine;

namespace DropThat.Drop.Options.Modifiers;

public class ModifierQualityLevel : IItemModifier
{
    public int? QualityLevel { get; set; }

    public ModifierQualityLevel() { }

    public ModifierQualityLevel(int? qualityLevel)
    {
        QualityLevel = qualityLevel;
    }

    public void Modify(GameObject drop)
    {
        if (QualityLevel is null ||
            QualityLevel < 0)
        {
            return;
        }

        var itemDrop = ComponentCache.Get<ItemDrop>(drop);

        if (itemDrop.IsNull())
        {
            return;
        }

        Log.Warning?.Log($"Setting quality level of item '{itemDrop.name}' to {QualityLevel}.");
        itemDrop.m_itemData.m_durability = QualityLevel.Value;
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
            template.ItemModifiers.AddOrReplaceByType(new ModifierQualityLevel(qualityLevel.Value));
        }
        else
        {
            template.ItemModifiers.RemoveAll(x => x is ModifierQualityLevel);
        }

        return template;
    }
}