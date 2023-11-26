using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DropThat.Drop.Options;

public interface IItemModifier
{
    void Modify(ItemModifierContext<GameObject> drop);

    void Modify(ItemModifierContext<ItemDrop.ItemData> drop);
}

public static class IItemModifierExtensions
{
    public static T GetOrDefault<T>(this ICollection<IItemModifier> conditions) =>
        conditions.OfType<T>().FirstOrDefault();

    public static T GetOrCreate<T>(this ICollection<IItemModifier> conditions)
        where T : IItemModifier, new()
    {
        var existing = conditions.OfType<T>().FirstOrDefault();

        if (existing is not null)
        {
            return existing;
        }

        var newCondition = new T();

        conditions.Add(newCondition);

        return newCondition;
    }

    public static bool Remove<T>(this ICollection<IItemModifier> conditions)
    {
        for (int i = 0; i < conditions.Count; ++i)
        {
            var element = conditions.ElementAt(i);

            if (conditions.ElementAt(i) is T)
            {
                conditions.Remove(element);
                return true;
            }
        }

        return false;
    }
}