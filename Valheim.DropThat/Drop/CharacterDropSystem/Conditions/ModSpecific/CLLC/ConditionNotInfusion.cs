using System.Collections.Generic;
using System.Linq;
using CreatureLevelControl;
using DropThat.Integrations;
using DropThat.Integrations.CllcIntegration;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;

public class ConditionNotInfusion : IDropCondition
{
    public CllcCreatureInfusion[] Infusions { get; set; }

    public ConditionNotInfusion() { }

    public ConditionNotInfusion(IEnumerable<CllcCreatureInfusion> infusions)
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

        return !HasInfusion(context.Character);
    }

    private bool HasInfusion(Character character)
    {
        var currentInfusion = API.GetInfusionCreature(character);

        return Infusions.Any(x => x.Convert() == currentInfusion);
    }
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionNotInfusion(
        this CharacterDropDropTemplate template,
        IEnumerable<CllcCreatureInfusion> infusions)
    {
        if (infusions?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionNotInfusion(infusions));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionNotInfusion);
        }

        return template;
    }
}
