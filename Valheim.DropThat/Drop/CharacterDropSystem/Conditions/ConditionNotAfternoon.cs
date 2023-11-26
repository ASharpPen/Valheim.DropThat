using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionNotAfternoon : IDropCondition
{
    public bool IsPointless() => false;

    public bool IsValid(DropContext context) => !EnvMan.instance.IsAfternoon();
}