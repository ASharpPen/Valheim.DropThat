using System.Linq;
using UnityEngine;
using DropThat.Configuration.ConfigTypes;
using DropThat.Core;
using DropThat.Utilities;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionBiome : IDropTableCondition
{
    private static ConditionBiome _instance;

    public static ConditionBiome Instance => _instance ??= new();

    public bool ShouldFilter(DropSourceTemplateLink context, DropTemplate template)
    {
        if (IsValid(context.Source.transform.position, template?.Config))
        {
            return false;
        }

        Log.LogTrace($"Filtered drop '{template.Drop.m_item.name}' due being outside required biome.");
        return true;
    }

    public bool IsValid(Vector3 position, DropTableItemConfiguration config)
    {
        if (config is null)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(config.ConditionBiomes.Value))
        {
            return true;
        }

        var allowedBiomes = config.ConditionBiomes.Value.SplitByComma(true);

        var currentBiome = Heightmap.FindBiome(position);
        var currentBiomeCleaned = currentBiome.ToString().ToUpperInvariant();

        return allowedBiomes.Any(x => x == currentBiomeCleaned);
    }
}
