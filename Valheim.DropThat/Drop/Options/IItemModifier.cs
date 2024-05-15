using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DropThat.Drop.Options;

public interface IItemModifier
{
    void Modify(ItemModifierContext<GameObject> drop);

    void Modify(ItemModifierContext<ItemDrop.ItemData> drop);

    bool IsPointless();
}

public static class IItemModifierExtensions
{
    public static T GetOrDefault<T>(this ICollection<IItemModifier> modifiers) =>
        modifiers.OfType<T>().FirstOrDefault();
}