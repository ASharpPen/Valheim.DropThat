using CreatureLevelControl;
using System.Linq;
using DropThat.Integrations.CllcIntegration;
using System.Collections.Generic;
using ThatCore.Extensions;
using DropThat.Integrations;
using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;

public class ConditionCreatureExtraEffect : IDropCondition
{
    public CllcCreatureExtraEffect[] ExtraEffects { get; set; }

    public ConditionCreatureExtraEffect() { }

    public ConditionCreatureExtraEffect(IEnumerable<CllcCreatureExtraEffect> extraEffects) 
    {
        ExtraEffects = extraEffects.ToArray();
    }

    public bool IsValid(DropContext context)
    {
        if (ExtraEffects is null ||
            ExtraEffects.Length == 0 ||
            context.Character.IsNull())
        {
            return true;
        }

        if (!InstallationManager.CLLCInstalled)
        {
            return true;
        }

        return HasExtraEffect(context.Character);
    }

    private bool HasExtraEffect(Character character)
    {
        var creatureExtraEffect = API.GetExtraEffectCreature(character);

        return ExtraEffects.Any(x => x.Convert() == creatureExtraEffect);
    }
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionCreatureExtraEffect(
        this CharacterDropDropTemplate template,
        IEnumerable<CllcCreatureExtraEffect> creatureExtraEffects)
    {
        if (creatureExtraEffects?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionCreatureExtraEffect(creatureExtraEffects));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionCreatureExtraEffect);
        }

        return template;
    }
}
