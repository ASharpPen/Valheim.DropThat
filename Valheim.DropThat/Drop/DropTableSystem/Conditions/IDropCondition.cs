using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

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