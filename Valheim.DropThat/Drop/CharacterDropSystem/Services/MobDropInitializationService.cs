using System.Collections.Generic;
using System.Linq;
using DropThat.Configuration.ConfigTypes;

namespace DropThat.Drop.CharacterDropSystem.Services;

internal static class MobDropInitializationService
{
    public static IEnumerable<CharacterDropItemConfiguration> PrepareInsertion(CharacterDropListConfiguration listConfig, CharacterDropMobConfiguration mobConfig)
    {
        Dictionary<int, CharacterDropItemConfiguration> result = new();

        if (listConfig is not null)
        {
            foreach (var config in listConfig.Subsections.Values)
            {
                if (config.EnableConfig)
                {
                    result[config.Index] = config;
                }
            }
        }

        if (mobConfig is not null)
        {
            foreach (var config in mobConfig.Subsections.Values)
            {
                if (config.EnableConfig)
                {
                    result[config.Index] = config;
                }
            }
        }

        return result.Values.OrderBy(x => x.Index);
    }
}
