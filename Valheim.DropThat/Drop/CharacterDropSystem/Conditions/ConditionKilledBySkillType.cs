using System.Collections.Generic;
using System.Linq;
using DropThat.Creature.DamageRecords;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionKilledBySkillType : IDropCondition
{
    public HashSet<Skills.SkillType> SkillTypes { get; }

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

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionKilledBySkillType(
        this CharacterDropDropTemplate template,
        IEnumerable<Skills.SkillType> skillTypes)
    {
        if (skillTypes?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionKilledBySkillType(skillTypes));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionKilledBySkillType);
        }

        return template;
    }
}