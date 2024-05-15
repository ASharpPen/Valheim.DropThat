using System.Linq;
using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public sealed class ConditionGlobalKeysAny : IDropCondition
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

        return GlobalKeys.Any(ZoneSystem.instance.GetGlobalKey);
    }
}