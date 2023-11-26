using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public sealed class ConditionDistanceToCenterMax : IDropCondition
{
    public float DistanceToCenterMax { get; set; }

    public bool IsPointless() => DistanceToCenterMax <= 0;

    public bool IsValid(DropContext context)
    {
        var distance = context.ZDO.m_position.magnitude;

        return distance <= DistanceToCenterMax;
    }
}