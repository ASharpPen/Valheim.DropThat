using CreatureLevelControl;
using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions.ModSpecific.CLLC;

public sealed class ConditionWorldLevelMax : IDropCondition
{
    public int? WorldLevel { get; set; }

    public bool IsPointless() =>
        WorldLevel is null ||
        WorldLevel <= 0;

    public bool IsValid(DropContext context) => 
        WorldLevel is null ||
        WorldLevel <= API.GetWorldLevel();
}