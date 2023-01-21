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

    public ConditionInfusion() { }

    public ConditionInfusion(IEnumerable<CllcCreatureInfusion> infusions) 
    {
        Infusions = infusions.ToArray();
    }

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
            template.Conditions.AddOrReplaceByType(new ConditionInfusion(infusions));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionInfusion);
        }

        return template;
    }
}
