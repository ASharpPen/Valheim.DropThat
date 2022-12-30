using DropThat.Caches;
using DropThat.Core;

namespace DropThat.Drop.CharacterDropSystem.Modifiers;

public class ModifierSetQualityLevel : IDropModifier
{
    private static ModifierSetQualityLevel _instance;

    public static ModifierSetQualityLevel Instance => _instance ??= new();

    public void Modify(DropContext context)
    {
        if (context?.Item is null)
        {
            return;
        }

        if (context?.Extended?.Config?.SetQualityLevel <= 0)
        {
            return;
        }

        var itemDrop = ComponentCache.Get<ItemDrop>(context.Item);

        Log.LogTrace($"Setting level of item '{context.Item.name}' to {context.Extended.Config.SetQualityLevel.Value}");
        itemDrop.m_itemData.m_quality = context.Extended.Config.SetQualityLevel;
    }
}
