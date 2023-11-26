using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public sealed class ConditionNotDay : IDropCondition
{
    public bool IsPointless() => false;

    public bool IsValid(DropContext context) => !EnvMan.instance.IsDay();
}