using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Drop.CharacterDropSystem.Services;
using ThatCore.Extensions;
using ThatCore.Lifecycle;
using ThatCore.Logging;
using UnityEngine;

namespace DropThat.Drop.CharacterDropSystem.Managers;

public static class CharacterDropTemplateManager
{
    internal static Dictionary<string, CharacterDropMobTemplate> Templates { get; set; } = new();

    static CharacterDropTemplateManager()
    {
        LifecycleManager.OnWorldInit += () =>
        {
            Templates = new();
        };
    }

    public static void SetTemplate(string prefabName, CharacterDropMobTemplate template) =>
        Templates[prefabName] = template;

    public static List<CharacterDropMobTemplate> GetTemplates() => Templates.Values.ToList();

    public static CharacterDropMobTemplate GetTemplate(string prefabName)
    {
        if (Templates.TryGetValue(prefabName, out var template))
        {
            return template;
        }

        return null;
    }

    public static bool TryGetTemplate(string prefabName, out CharacterDropMobTemplate template) =>
        Templates.TryGetValue(prefabName, out template);

    public static bool TryGetTemplate(string prefabName, int id, out CharacterDropDropTemplate template)
    {
        if (Templates.TryGetValue(prefabName, out var mobTemplate))
        {
            return mobTemplate.Drops.TryGetValue(id, out template);
        }

        template = null;
        return false;
    }

    /// <summary>
    /// Override existing templates with a new set.
    /// Reset all currently loaded creatures.
    /// </summary>
    internal static void ResetTemplates(IEnumerable<CharacterDropMobTemplate> templates)
    {
        Log.Development?.Log("Resetting CharacterDrop templates");

        Templates = templates.ToDictionary(x => x.PrefabName);

#if !TEST
        // Reset currently loaded creatures, so that drop tables can be re-applied.
        foreach (var instance in CharacterDropSessionManager.CharacterDropInstances.Values)
        {
            Log.Development?.Log($"Attempting to destroy '{instance.GetName()}'");

            if (instance.IsNotNull())
            {
                // Delete gameobject, and leave it to ZnetScene to re-instantiate it.
                // Replicates behaviour of ZnetScene.RemoveObjects removing objects far away.
                var znetView = instance.GetComponent<ZNetView>();
                ZDO zdo = znetView.GetZDO();
                
                // Remove ZDO reference from ZNetView
                znetView.ResetZDO();

                // Delete gameobject instance
                Object.Destroy(znetView.gameObject);

                // Delete link to now dead ZNetView
                ZNetScene.instance.m_instances.Remove(zdo);
            }
        }
#endif
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
    public static Dictionary<string, ExpectedCharacterDrop> GetAllExpectedDropTables()
    {
        return DropExpectationService.AllExpectedDrops();
    }

    /// <summary>
    /// <para>
    ///     Returns the expected drop tables after <see cref="CharacterDropMobTemplate"/> 
    ///     (if any) has been applied.
    /// </para>
    /// <para>
    ///     This show default drops if no templates existed for prefab.
    /// </para>
    /// </summary>
    /// <exception cref="InvalidOperationException">If accessed prior to ZnetScene being instantiated.</exception>
    public static bool TryGetExpectedDrops(string prefab, out ExpectedCharacterDrop expectedDropTable) =>
        DropExpectationService.TryGetExpectedDrops(prefab, out expectedDropTable);

}
