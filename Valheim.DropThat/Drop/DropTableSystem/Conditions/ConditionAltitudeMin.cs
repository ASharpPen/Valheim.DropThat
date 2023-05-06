using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionAltitudeMin : IDropCondition
{
    public float? AltitudeMin { get; set; }

    public ConditionAltitudeMin() { }

    public ConditionAltitudeMin(float? altitudeMin = null)
    {
        AltitudeMin = altitudeMin;
    }

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

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionAltitudeMin(
        this IHaveDropConditions template,
        float? altitude)
    {
        if (altitude is not null)
        {
            template.Conditions.GetOrCreate<ConditionAltitudeMin>().AltitudeMin = altitude;
        }
        else
        {
            template.Conditions.Remove<ConditionAltitudeMin>();
        }

        return template;
    }
}