using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Drop.CharacterDropSystem.Services;

namespace DropThat.Drop.CharacterDropSystem.Managers;

public static class CharacterDropManager
{
    private static Dictionary<string, CharacterDropMobTemplate> Cache { get; } = new();
    private static Dictionary<string, CharacterDropMobTemplate> CacheConfigured { get; } = new();

    private static bool CachedAll { get; set; }
    private static bool CachedConfigured { get; set; }

    static CharacterDropManager()
    {
        DropSystemConfigManager.OnConfigsLoadedEarly += () =>
        {
            // Reset cached compiled drops when configs are reloaded.
            Cache.Clear();
            CacheConfigured.Clear();
            CachedAll = false;
            CachedConfigured = false;
        };
    }

    /// <summary>
    /// <para>
    ///     Scans all prefabs and returns the expected drop tables after <see cref="CharacterDropMobTemplate"/>s
    ///     (if any) have been applied.
    /// </para>
    /// <para>
    ///     This will include default drops if no templates existed.
    /// </para> 
    /// </summary>
    /// <exception cref="InvalidOperationException">If accessed prior to ZnetScene being instantiated.</exception>
    public static List<CharacterDropMobTemplate> CompileAllPrefabDrops()
    {
        if (CachedAll)
        {
            return Cache.Values.ToList();
        }

        foreach (var entry in DropTableCompiler.CompileAllDrops())
        {
            Cache[entry.PrefabName] = entry;
        }

        CachedAll = true;

        return Cache.Values.ToList();
    }

    /// <summary>
    /// <para>
    ///     Returns the expected drop tables after <see cref="CharacterDropMobTemplate"/> 
    ///     (if any) has been applied.
    /// </para>
    /// <para>
    ///     Only returns drop tables for entities that had configs associated.
    /// </para>
    /// </summary>
    /// <exception cref="InvalidOperationException">If accessed prior to ZnetScene being instantiated.</exception>
    public static List<CharacterDropMobTemplate> CompileConfiguredPrefabDrops()
    {
        if (CachedConfigured)
        {
            return CacheConfigured.Values.ToList();
        }

        foreach (var prefabName in CharacterDropTemplateManager.Templates.Keys)
        {
            if (DropTableCompiler.TryCompileDrops(
                prefabName, 
                applyTemplate: true, 
                out var compiledDrops))
            {
                Cache[prefabName] = compiledDrops;
                CacheConfigured[prefabName] = compiledDrops;
            }
        }

        CachedConfigured = true;

        return CacheConfigured.Values.ToList();
    }

    /// <summary>
    /// <para>
    ///     Returns the expected drop tables after <see cref="CharacterDropMobTemplate"/> 
    ///     (if any) has been applied.
    /// </para>
    /// <para>
    ///     This show default drops if no templates existed for prefab.
    /// </para>
    /// <para>
    ///     Returns false if prefab could not be found, or if it had no CharacterDrop component.
    /// </para>
    /// </summary>
    /// <exception cref="InvalidOperationException">If accessed prior to ZnetScene being instantiated.</exception>
    public static bool TryCompileDrops(
        string prefabName, 
        out CharacterDropMobTemplate compiledDrops)
    {
        if (Cache.TryGetValue(prefabName, out compiledDrops))
        {
            return true;
        }

        if (DropTableCompiler.TryCompileDrops(
            prefabName, 
            applyTemplate: true, 
            out compiledDrops))
        {
            Cache[prefabName] = compiledDrops;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns the expected drop tables, without applying any associated configs.
    /// </summary>
    public static List<CharacterDropMobTemplate> CompileWithoutTemplates() => 
        DropTableCompiler.CompileAllDrops(applyTemplate: false);

    public static bool TryCompileWithoutTemplates(
        string prefabName, 
        out CharacterDropMobTemplate compiledDrops)
    {
        if (DropTableCompiler.TryCompileDrops(
            prefabName,
            applyTemplate: false,
            out compiledDrops))
        {
            return true;
        }

        return false;
    }
}
