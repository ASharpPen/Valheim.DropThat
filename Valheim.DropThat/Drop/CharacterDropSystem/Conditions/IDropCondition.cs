using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public interface IDropCondition
{
    bool IsValid(DropContext context);

    bool IsPointless();
}

public static class IDropConditionExtensions
{
    public static T GetOrDefault<T>(this ICollection<IDropCondition> conditions) => 
        conditions.OfType<T>().FirstOrDefault();
}