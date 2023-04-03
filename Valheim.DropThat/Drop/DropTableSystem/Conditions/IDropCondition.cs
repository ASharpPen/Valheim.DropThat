using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public interface IDropCondition
{
    bool IsValid(DropContext context);
}
