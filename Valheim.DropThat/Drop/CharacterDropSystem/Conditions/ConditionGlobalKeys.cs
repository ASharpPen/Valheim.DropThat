using DropThat.Core;
using DropThat.Drop.CharacterDropSystem.Caches;
using DropThat.Utilities;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

internal class ConditionGlobalKeys : ICondition
{
    private static ConditionGlobalKeys _instance;

    public static ConditionGlobalKeys Instance => _instance ??= new();

    public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended dropExtended, CharacterDrop characterDrop)
    {
        if (!string.IsNullOrEmpty(dropExtended.Config.ConditionGlobalKeys.Value))
        {
            var requiredKeys = dropExtended.Config.ConditionGlobalKeys.Value.SplitByComma();

            if (requiredKeys.Count > 0)
            {
                bool foundRequiredKey = false;

                foreach (var key in requiredKeys)
                {
                    foundRequiredKey = ZoneSystem.instance.GetGlobalKey(key);

                    if (foundRequiredKey)
                    {
                        break;
                    }
                }

                if (!foundRequiredKey)
                {
                    Log.LogTrace($"{nameof(dropExtended.Config.ConditionGlobalKeys)}: Disabling drop {drop.m_prefab.name} due to not finding any of the requires global keys '{dropExtended.Config.ConditionGlobalKeys.Value}'.");

                    return true;
                }
            }
        }

        return false;
    }
}
