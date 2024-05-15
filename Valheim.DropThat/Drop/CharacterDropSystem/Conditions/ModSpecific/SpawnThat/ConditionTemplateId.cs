using System.Linq;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Integrations;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.SpawnThat;

public sealed class ConditionTemplateId : IDropCondition
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