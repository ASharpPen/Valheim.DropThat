using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public sealed class ConditionDistanceToCenterMin : IDropCondition
{
    public float DistanceToCenterMin { get; set; }

    public bool IsPointless() => DistanceToCenterMin <= 0;

    public bool IsValid(DropContext context)
    {
        var distance = context.ZDO.m_position.magnitude;

        return distance >= DistanceToCenterMin;
    }
}