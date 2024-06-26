﻿using System.Linq;
using CreatureLevelControl;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Integrations;
using DropThat.Integrations.CllcIntegration;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;

public sealed class ConditionNotCreatureExtraEffect : IDropCondition
{
    public CllcCreatureExtraEffect[] ExtraEffects { get; set; }

    public bool IsPointless() => (ExtraEffects?.Length ?? 0) == 0;

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

        return !HasExtraEffect(context.Character);
    }

    private bool HasExtraEffect(Character character)
    {
        var creatureExtraEffect = API.GetExtraEffectCreature(character);

        return ExtraEffects.Any(x => x.Convert() == creatureExtraEffect);
    }
}