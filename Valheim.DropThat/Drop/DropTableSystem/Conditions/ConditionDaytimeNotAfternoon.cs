using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionDaytimeNotAfternoon : IDropCondition
{
    public bool IsPointless() => false;

    public bool IsValid(DropContext context) =>
        !EnvMan.instance.IsAfternoon();
}