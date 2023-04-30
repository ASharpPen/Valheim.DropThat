using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

internal static class IDropConditionExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDropCondition FirstOfType<TDropCondition>(this List<IDropCondition> list)
        where TDropCondition : IDropCondition =>
        list.OfType<TDropCondition>().FirstOrDefault();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDropCondition FindCondition<TDropCondition>(this IHaveDropConditions haveDropConditions)
        where TDropCondition : IDropCondition =>
        haveDropConditions.Conditions.OfType<TDropCondition>().FirstOrDefault();

}
