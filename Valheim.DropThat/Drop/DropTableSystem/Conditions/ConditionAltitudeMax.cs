using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionAltitudeMax : IDropCondition
{
    public float? AltitudeMax { get; set; }

    public bool IsPointless() => AltitudeMax is null;

    public bool IsValid(DropContext context)
    {
        if (AltitudeMax is null)
        {
            return true;
        }

        var altitude = context.Pos.y - ZoneSystem.instance.m_waterLevel;

        return altitude <= AltitudeMax;
    }
}