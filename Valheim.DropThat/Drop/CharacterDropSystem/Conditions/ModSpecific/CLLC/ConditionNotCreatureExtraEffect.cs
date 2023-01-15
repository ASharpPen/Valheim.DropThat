﻿using System.Collections.Generic;
using System.Linq;
using CreatureLevelControl;
using DropThat.Integrations.CllcIntegration;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;

public class ConditionNotCreatureExtraEffect : IDropCondition
{
    public CllcCreatureExtraEffect[] ExtraEffects { get; set; }

    public ConditionNotCreatureExtraEffect() { }

    public ConditionNotCreatureExtraEffect(IEnumerable<CllcCreatureExtraEffect> extraEffects)
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

        if (!ConditionLoaderCLLC.InstalledCLLC)
        {
            return true;
        }

        return !HasExtraEffect(context.Character);
    }

    private bool HasExtraEffect(Character character)
    {
        var creatureExtraEffect = API.GetExtraEffectCreature(character);

        return ExtraEffects.Any(x => x.Convert() == creatureExtraEffect);
    }
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionNotCreatureExtraEffect(
        this CharacterDropDropTemplate template,
        IEnumerable<CllcCreatureExtraEffect> creatureExtraEffects)
    {
        if (creatureExtraEffects?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionNotCreatureExtraEffect(creatureExtraEffects));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionNotCreatureExtraEffect);
        }

        return template;
    }
}

