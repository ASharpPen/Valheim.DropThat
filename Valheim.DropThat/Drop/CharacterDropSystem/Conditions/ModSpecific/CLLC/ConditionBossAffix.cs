using CreatureLevelControl;
using System.Linq;
using DropThat.Integrations.CllcIntegration;
using System.Collections.Generic;
using ThatCore.Extensions;
using DropThat.Integrations;
using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;

public class ConditionBossAffix : IDropCondition
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

        return HasAffix(context.Character);
    }

    private bool HasAffix(Character character)
    {
        var currentBossAffix = API.GetAffixBoss(character);

        return BossAffixes.Any(x => x.Convert() == currentBossAffix);
    }
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionBossAffix(
        this IHaveDropConditions template,
        IEnumerable<CllcBossAffix> bossAffixes)
    {
        if (bossAffixes?.Any() == true)
        {
            template.Conditions
                .GetOrCreate<ConditionBossAffix>()
                .BossAffixes = bossAffixes.ToArray();
        }
        else
        {
            template.Conditions.Remove<ConditionBossAffix>();
        }

        return template;
    }
}
