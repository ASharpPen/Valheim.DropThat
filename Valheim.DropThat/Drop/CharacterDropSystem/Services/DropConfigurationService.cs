using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;
using ThatCore.Logging;
using ThatCore.Models;
using UnityEngine;

namespace DropThat.Drop.CharacterDropSystem.Services;

internal static class DropConfigurationService
{
    public static bool TryConfigureNewDrop(DropConfigInfo context, out CharacterDrop.Drop drop)
    {
        var template = context.DropTemplate;

        if (!TryFindPrefab(template.PrefabName, out var prefab))
        {
            drop = null;
            Log.Warning?.Log($"{context.DisplayName} Unable to find prefab '{template.PrefabName}'. Skipping insert of drop.");
            return false;
        }

        drop = new()
        {
            m_prefab = prefab,
            m_amountMax = template.AmountMax.Value.GetValueOrDefault(),
            m_amountMin = template.AmountMin.Value.GetValueOrDefault(),
            m_chance = template.ChanceToDrop.Value.GetValueOrDefault() / 100,
            m_levelMultiplier = template.ScaleByLevel.Value.GetValueOrDefault(),
            m_onePerPlayer = template.DropOnePerPlayer.Value.GetValueOrDefault(),
        };

        return true;
    }

    public static void ConfigureExistingDrop(DropConfigInfo context, CharacterDrop.Drop drop)
    {
        var template = context.DropTemplate;
        var existingDrop = drop.m_prefab.GetCleanedName();

        if (!string.IsNullOrWhiteSpace(template.PrefabName) &&
            existingDrop != template.PrefabName)
        {
            if (!TryFindPrefab(template.PrefabName, out var prefab))
            {
                Log.Warning?.Log($"{context.DisplayName} Unable to find prefab '{template.PrefabName}'.");
            }
            else
            {
                drop.m_prefab = prefab;
            }
        }

        template.AmountMax.SetIfNotNull(ref drop.m_amountMax);
        template.AmountMin.SetIfNotNull(ref drop.m_amountMin);
        template.ScaleByLevel.SetIfNotNull(ref drop.m_levelMultiplier);
        template.DropOnePerPlayer.SetIfNotNull(ref drop.m_onePerPlayer);

        if (template.ChanceToDrop.IsSet &&
            template.ChanceToDrop.Value is not null)
        {
            drop.m_chance = template.ChanceToDrop.Value.Value / 100f;
        }
    }

    private static bool TryFindPrefab(string prefabName, out GameObject prefab)
    {
        prefab = ObjectDB.instance.GetItemPrefab(prefabName);

        if (prefab.IsNull())
        {
            prefab = ZNetScene.instance.GetPrefab(prefabName);
        }

        if (prefab.IsNull())
        {
            return false;
        }

        return true;
    }

    private static void SetIfNotNull<T>(this Optional<T?> source, ref T dest)
        where T : struct
    {
        if (source.IsSet &&
            source.Value is not null)
        {
            dest = source.Value.Value;
        }
    }
}
