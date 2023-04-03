using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionAltitudeMax : IDropCondition
{
    public float? AltitudeMax { get; set; }

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
            template.Conditions.AddOrReplaceByType(new ConditionAltitudeMax(altitude));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionAltitudeMax);
        }

        return template;
    }
}