using Valheim.DropThat.Caches;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Drop.CharacterDropSystem.Modifiers;

public class ModifierSetDurability : IDropModifier
{
    private static ModifierSetDurability _instance;

    public static ModifierSetDurability Instance => _instance ??= new();

    public void Modify(DropContext context)
    {
        if (context?.Item is null)
        {
            return;
        }

        if (context?.Extended?.Config?.SetDurability < 0)
        {
            return;
        }

        var itemDrop = ComponentCache.GetComponent<ItemDrop>(context.Item);

        Log.LogTrace($"Setting durability of item '{context.Item.name}' to {context.Extended.Config.SetDurability.Value}");
        itemDrop.m_itemData.m_durability = context.Extended.Config.SetDurability;
    }
}
