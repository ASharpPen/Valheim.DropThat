using System.Linq;
using DropThat.Caches;
using DropThat.Configuration.ConfigTypes;
using DropThat.Core;
using DropThat.Core.Configuration;
using DropThat.Drop.CharacterDropSystem.Caches;
using DropThat.Utilities;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.SpawnThat;

public class ConditionTemplateId : ICondition
{
    private static ConditionTemplateId _instance;

    public static ConditionTemplateId Instance
    {
        get
        {
            return _instance ??= new ConditionTemplateId();
        }
    }

    public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop)
    {
        if (!extended.Config.Subsections.TryGetValue(CharacterDropModConfigSpawnThat.ModName, out Config modConfig))
        {
            return false;
        }

        var config = modConfig as CharacterDropModConfigSpawnThat;

        if (config is null || string.IsNullOrWhiteSpace(config.ConditionTemplateId.Value))
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

        var configTemplateIds = config.ConditionTemplateId.Value.SplitByComma();

        if (!configTemplateIds.Any(x => x == templateId))
        {
            Log.LogTrace($"{nameof(config.ConditionTemplateId)}: Disabling drop {drop.m_prefab.name} due to not having required spawn template id {config.ConditionTemplateId.Value}.");
            return true;
        }

        return false;
    }
}
