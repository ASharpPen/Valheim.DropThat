using System.Collections.Generic;
using System.Linq;
using DropThat.Creature.DamageRecords;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionKilledBySkillType : IDropCondition
{
    public HashSet<Skills.SkillType> SkillTypes { get; set; }

    public ConditionKilledBySkillType() { }

    public ConditionKilledBySkillType(IEnumerable<Skills.SkillType> skillTypes)
    {
        SkillTypes = skillTypes.ToHashSet();
    }

    public bool IsValid(DropContext context)
    {
        if (SkillTypes is null ||
            SkillTypes.Count == 0)
        {
            return true;
        }

        DamageRecord lastHit = RecordLastHit.GetLastHit(context.Character);

        if (lastHit is null)
        {
            return false;
        }

        return SkillTypes.Contains(lastHit.SkillType);
    }
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionKilledBySkillType(
        this IHaveDropConditions template,
        IEnumerable<Skills.SkillType> skillTypes)
    {
        if (skillTypes?.Any() == true)
        {
            template.Conditions
                .GetOrCreate<ConditionKilledBySkillType>()
                .SkillTypes = skillTypes.ToHashSet();
        }
        else
        {
            template.Conditions.Remove<ConditionKilledBySkillType>();
        }

        return template;
    }
}