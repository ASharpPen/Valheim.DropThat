using System.Linq;
using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionGlobalKeysNotAll : IDropCondition
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

        return !GlobalKeys.All(ZoneSystem.instance.GetGlobalKey);
    }
}