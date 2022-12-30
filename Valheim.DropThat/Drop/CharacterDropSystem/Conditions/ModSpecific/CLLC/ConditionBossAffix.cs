using CreatureLevelControl;
using System;
using System.Linq;
using DropThat.Caches;
using DropThat.Configuration.ConfigTypes;
using DropThat.Core;
using DropThat.Core.Configuration;
using DropThat.Drop.CharacterDropSystem.Caches;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;

internal class ConditionBossAffix : ICondition
{
    private static ConditionBossAffix _instance;

    public static ConditionBossAffix Instance
    {
        get
        {
            return _instance ??= new ConditionBossAffix();
        }
    }

    public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop)
    {
        if (extended?.Config?.Subsections is null)
        {
            return false;
        }

        var character = CharacterCache.GetCharacter(characterDrop);

        if (!character.IsBoss())
        {
            return false;
        }

        if (extended.Config.Subsections.TryGetValue(CharacterDropModConfigCLLC.ModName, out Config config) && config is CharacterDropModConfigCLLC cllcConfig)
        {
            if (!ValidConditionBossAffix(drop, cllcConfig, character))
            {
                return true;
            }

            if (!ValidConditionNotBossAffix(drop, cllcConfig, character))
            {
                return true;
            }
        }

        return false;
    }

    public static bool ValidConditionBossAffix(CharacterDrop.Drop drop, CharacterDropModConfigCLLC config, Character character)
    {
        if (config.ConditionBossAffix.Value.Length > 0)
        {
            var states = config.ConditionBossAffix.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if ((states?.Length ?? 0) == 0)
            {
#if DEBUG
                Log.LogDebug("No conditions for CLLC boss affixes were found.");
#endif
                //Skip if we have no states to check. This indicates all are allowed.
                return true;
            }

            if (!states.Any(x => HasState(character, x)))
            {
                Log.LogTrace($"{nameof(config.ConditionBossAffix)}: Disabling drop {drop.m_prefab.name} due to not finding any of the requires affixes '{config.ConditionBossAffix.Value}'.");
                return false;
            }
        }

        return true;
    }

    public static bool ValidConditionNotBossAffix(CharacterDrop.Drop drop, CharacterDropModConfigCLLC config, Character character)
    {
        if (config.ConditionNotBossAffix.Value.Length > 0)
        {
            var states = config.ConditionNotBossAffix.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if ((states?.Length ?? 0) == 0)
            {
#if DEBUG
                Log.LogDebug("No conditions for CLLC boss affixes were found.");
#endif
                //Skip if we have no states to check. This indicates all are allowed.
                return true;
            }

            if (states.Any(x => HasState(character, x)))
            {
                Log.LogTrace($"{nameof(config.ConditionNotBossAffix)}: Disabling drop {drop.m_prefab.name} due finding one of the disabled affixes '{config.ConditionNotBossAffix.Value}'.");
                return false;
            }
        }

        return true;
    }

    private static bool HasState(Character character, string state)
    {
        if (Enum.TryParse(state.Trim(), true, out BossAffix bossAffix))
        {
            return API.GetAffixBoss(character) == bossAffix;
        }
        else
        {
            Log.LogWarning($"Unable to parse CLLC boss affix '{state}'");
            return false;
        }
    }
}
