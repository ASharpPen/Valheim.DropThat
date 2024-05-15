using System.Linq;
using DropThat.Caches;
using DropThat.Drop.DropTableSystem.Models;
using DropThat.Integrations;

namespace DropThat.Drop.DropTableSystem.Conditions.ModSpecific.SpawnThat;

public sealed class ConditionTemplateId : IDropCondition
{
    public string[] TemplateIds { get; set; }

    public bool IsPointless() => (TemplateIds?.Length ?? 0) == 0;

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