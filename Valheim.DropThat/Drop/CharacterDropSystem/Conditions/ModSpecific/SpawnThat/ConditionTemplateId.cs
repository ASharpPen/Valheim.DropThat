using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Integrations;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.SpawnThat;

public class ConditionTemplateId : IDropCondition
{
    public string[] TemplateIds { get; set; }

    public bool IsPointless() => (TemplateIds?.Length ?? 0) == 0;

    public bool IsValid(DropContext context)
    {
        if (!InstallationManager.SpawnThatInstalled ||
            TemplateIds is null ||
            TemplateIds.Length == 0 ||
            context.ZDO is null)
        {
            return true;
        }

        var templateId = context.ZDO.GetString("spawn_template_id", null);

        return TemplateIds.Contains(templateId);
    }
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionTemplateId(
        this IHaveDropConditions template,
        IEnumerable<string> ids)
    {
        if (ids?.Any() == true)
        {
            template.Conditions
                .GetOrCreate<ConditionTemplateId>()
                .TemplateIds = ids.ToArray();
        }
        else
        {
            template.Conditions.Remove<ConditionTemplateId>();
        }

        return template;
    }
}
