using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public sealed class ConditionDaytimeNotAfternoon : IDropCondition
{
    public bool IsPointless() => false;

    public bool IsValid(DropContext context) =>
        !EnvMan.IsAfternoon();
}