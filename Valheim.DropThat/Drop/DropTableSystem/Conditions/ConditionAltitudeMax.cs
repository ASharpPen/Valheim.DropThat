using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionAltitudeMax : IDropCondition
{
    public float? AltitudeMax { get; set; }

    public ConditionAltitudeMax(){ }

    public ConditionAltitudeMax(float? altitudeMax = null)
    {
        AltitudeMax = altitudeMax;
    }

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

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionAltitudeMax(
        this IHaveDropConditions template,
        float? altitude)
    {
        if (altitude is not null)
        {
            template.Conditions.GetOrCreate<ConditionAltitudeMax>().AltitudeMax = altitude;
        }
        else
        {
            template.Conditions.Remove<ConditionAltitudeMax>();
        }

        return template;
    }
}