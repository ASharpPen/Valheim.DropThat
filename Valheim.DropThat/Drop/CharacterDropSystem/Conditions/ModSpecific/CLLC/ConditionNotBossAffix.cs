﻿using System.Linq;
using CreatureLevelControl;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Integrations;
using DropThat.Integrations.CllcIntegration;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;

public sealed class ConditionNotBossAffix : IDropCondition
{
    public CllcBossAffix[] BossAffixes { get; set; }

    public bool IsPointless() => (BossAffixes?.Length ?? 0) == 0;

    public bool IsValid(DropContext context)
    {
        if (BossAffixes is null ||
            BossAffixes.Length == 0 ||
            context.Character.IsNull() ||
            !context.Character.IsBoss())
        {
            return true;
        }

        if (!InstallationManager.CLLCInstalled)
        {
            return true;
        }

        return !HasAffix(context.Character);
    }

    private bool HasAffix(Character character)
    {
        var currentBossAffix = API.GetAffixBoss(character);

        return BossAffixes.Any(x => x.Convert() == currentBossAffix);
    }
}