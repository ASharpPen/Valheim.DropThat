using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DropThat.Drop.Options;

internal static class ItemModifierExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TModifier FirstOfType<TModifier>(this List<IItemModifier> list)
        where TModifier : IItemModifier =>
        list.OfType<TModifier>().FirstOrDefault();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TModifier FindModifier<TModifier>(this IHaveItemModifiers haveItemModifiers)
        where TModifier : IItemModifier =>
        haveItemModifiers.ItemModifiers.OfType<TModifier>().FirstOrDefault();
}
