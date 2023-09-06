using CreatureLevelControl;
using System.Linq;
using DropThat.Integrations.CllcIntegration;
using System.Collections.Generic;
using ThatCore.Extensions;
using DropThat.Integrations;
using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;

public class ConditionInfusion : IDropCondition
{
    public CllcCreatureInfusion[] Infusions { get; set; }

    public bool IsPointless() => (Infusions?.Length ?? 0) == 0;

    public bool IsValid(DropContext context)
    {
        if (Infusions is null ||
            Infusions.Length == 0 ||
            context.Character.IsNull())
        {
            return true;
        }

        if (!InstallationManager.CLLCInstalled)
        {
            return true;
        }

        return HasInfusion(context.Character);
    }

    private bool HasInfusion(Character character)
    {
        var currentInfusion = API.GetInfusionCreature(character);

        return Infusions.Any(x => x.Convert() == currentInfusion);
    }
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionInfusion(
        this IHaveDropConditions template,
        IEnumerable<CllcCreatureInfusion> infusions)
    {
        if (infusions?.Any() == true)
        {
            template.Conditions
                .GetOrCreate<ConditionInfusion>()
                .Infusions = infusions.ToArray();
        }
        else
        {
            template.Conditions.Remove<ConditionInfusion>();
        }

        return template;
    }
}
