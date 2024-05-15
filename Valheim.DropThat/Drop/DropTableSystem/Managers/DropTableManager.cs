using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Models;
using DropThat.Drop.DropTableSystem.Services;

namespace DropThat.Drop.DropTableSystem.Managers;

public static class DropTableManager
{
    private static Dictionary<string, DropTableTemplate> Cache { get; } = new();
    private static Dictionary<string, DropTableTemplate> CacheConfigured { get; } = new();

    private static bool CachedAll { get; set; }
    private static bool CachedConfigured { get; set; }

    static DropTableManager()
    {
        DropSystemConfigManager.OnConfigsLoadedEarly += () =>
        {
            Cache.Clear();
            CacheConfigured.Clear();
            CachedAll = false;
            CachedConfigured = false;
        };
    }

    /// <summary>
    /// <para>
    ///     Scans all prefabs and returns the expected drop tables after <see cref="DropTableTemplate"/>s
    ///     (if any) have been applied.
    /// </para>
    /// <para>
    ///     This will include default drops if no templates existed.
    /// </para> 
    /// </summary>
    /// <exception cref="InvalidOperationException">If accessed prior to ZnetScene being instantiated.</exception>
    public static List<DropTableTemplate> CompileAllPrefabDrops()
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
    ///     Returns the expected drop tables after <see cref="DropTableTemplate"/> 
    ///     (if any) has been applied.
    /// </para>
    /// <para>
    ///     Only returns drop tables for entities that had configs associated.
    /// </para>
    /// </summary>
    /// <exception cref="InvalidOperationException">If accessed prior to ZnetScene being instantiated.</exception>
    public static List<DropTableTemplate> CompiledConfiguredPrefabDrops()
    {
        if (CachedConfigured)
        {
            return CacheConfigured.Values.ToList();
        }

        foreach (var prefabName in DropTableTemplateManager.Templates.Keys)
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
    ///     Returns the expected drop tables after <see cref="DropTableTemplate"/> 
    ///     (if any) has been applied.
    /// </para>
    /// <para>
    ///     This show default drops if no templates existed for prefab.
    /// </para>
    /// <para>
    ///     Returns false if prefab could not be found, or if it had no component with a DropTable.
    /// </para>
    /// </summary>
    /// <exception cref="InvalidOperationException">If accessed prior to ZnetScene being instantiated.</exception>

    public static bool TryCompileDrops(string prefabName, out DropTableTemplate compiledDrops)
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
    public static List<DropTableTemplate> CompileWithoutTemplates() =>
        DropTableCompiler.CompileAllDrops(applyTemplate: false);

    public static bool TryCompileWithoutTemplates(
        string prefabName,
        out DropTableTemplate compiledDrops)
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
