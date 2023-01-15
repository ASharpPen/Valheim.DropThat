namespace DropThat.Drop.CharacterDropSystem.Conditions;

public interface IDropCondition
{
    bool IsValid(DropContext context);
}
