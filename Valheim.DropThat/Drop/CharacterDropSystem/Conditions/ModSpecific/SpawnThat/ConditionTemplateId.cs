using System.Linq;
using DropThat.Caches;
using DropThat.Core;
using DropThat.Drop.CharacterDropSystem.Caches;
using DropThat.Drop.CharacterDropSystem.Configurations;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.SpawnThat;

public class ConditionTemplateId : ICondition
{
    private static ConditionTemplateId _instance;

    public static ConditionTemplateId Instance => _instance ??= new ConditionTemplateId();

    public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop)
    {
        if (!extended.Config.Subsections.TryGetValue(CharacterDropModConfigSpawnThat.ModName, out var modConfig))
        {
            return false;
        }

        var config = modConfig as CharacterDropModConfigSpawnThat;

        if ((config.ConditionTemplateId?.Value.Count ?? 0) == 0)
        {
            return false;
        }

        var character = CharacterCache.GetCharacter(characterDrop);
        if (!character || character is null)
        {
            return false;
        }

        var zdo = ZdoCache.GetZDO(character.gameObject);

        if (zdo is null)
        {
            return false;
        }

        var templateId = zdo.GetString("spawn_template_id", null);

        if (!config.ConditionTemplateId.Value.Any(x => x == templateId))
        {
            Log.LogTrace($"{nameof(config.ConditionTemplateId)}: Disabling drop {drop.m_prefab.name} due to not having required spawn template id {config.ConditionTemplateId.Value}.");
            return true;
        }

        return false;
    }
}
