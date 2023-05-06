using System.Collections.Generic;
using System.Linq;
using CreatureLevelControl;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Integrations;
using DropThat.Integrations.CllcIntegration;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;

public class ConditionNotBossAffix : IDropCondition
{
    public CllcBossAffix[] BossAffixes { get; set; }

    public ConditionNotBossAffix() { }

    public ConditionNotBossAffix(IEnumerable<CllcBossAffix> bossAffixes)
    {
        BossAffixes = bossAffixes.ToArray();
    }

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

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionNotBossAffix(
        this IHaveDropConditions template,
        IEnumerable<CllcBossAffix> bossAffixes)
    {
        if (bossAffixes?.Any() == true)
        {
            template.Conditions
                .GetOrCreate<ConditionNotBossAffix>()
                .BossAffixes = bossAffixes.ToArray();
        }
        else
        {
            template.Conditions.Remove<ConditionNotBossAffix>();
        }

        return template;
    }
}
