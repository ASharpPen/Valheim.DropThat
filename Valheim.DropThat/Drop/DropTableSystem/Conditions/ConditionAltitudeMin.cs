using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionAltitudeMin : IDropCondition
{
    public float? AltitudeMin { get; set; }

    public bool IsPointless() => AltitudeMin is null;

    public bool IsValid(DropContext context)
    {
        if (AltitudeMin is null)
        {
            return true;
        }

        var altitude = context.Pos.y - ZoneSystem.instance.m_waterLevel;

        return altitude >= AltitudeMin;
    }
}