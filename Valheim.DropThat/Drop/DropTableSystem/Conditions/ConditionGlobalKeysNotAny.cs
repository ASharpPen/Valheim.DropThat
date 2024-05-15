using System.Linq;
using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public sealed class ConditionGlobalKeysNotAny : IDropCondition
{
    public string[] GlobalKeys { get; set; }

    public bool IsPointless() => (GlobalKeys?.Length ?? 0) == 0;

    public bool IsValid(DropContext context)
    {
        if (GlobalKeys is null ||
            GlobalKeys.Length == 0)
        {
            return true;
        }

        return !GlobalKeys.Any(ZoneSystem.instance.GetGlobalKey);
    }
}