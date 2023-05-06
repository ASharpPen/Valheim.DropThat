using System.Collections.Generic;
using System.Linq;
using DropThat.Caches;
using DropThat.Drop.DropTableSystem.Models;
using DropThat.Integrations;
using ThatCore.Extensions;

namespace DropThat.Drop.DropTableSystem.Conditions.ModSpecific.SpawnThat;

public class ConditionTemplateId : IDropCondition
{
    public string[] TemplateIds { get; set; }

    public ConditionTemplateId() { }

    public ConditionTemplateId(IEnumerable<string> ids)
    {
        TemplateIds = ids.ToArray();
    }

    public bool IsValid(DropContext context)
    {
        if (!InstallationManager.SpawnThatInstalled ||
            TemplateIds is null ||
            TemplateIds.Length == 0)
        {
            return true;
        }

        var zdo = ZdoCache.GetZDO(context.DropTableSource);

        if (zdo is null)
        {
            return true;
        }


        var templateId = zdo.GetString("spawn_template_id", null);

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
