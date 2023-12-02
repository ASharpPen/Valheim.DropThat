using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Models;
using DropThat.Drop.DropTableSystem.Services;
using ThatCore.Extensions;
using ThatCore.Lifecycle;
using ThatCore.Logging;
using UnityEngine;

namespace DropThat.Drop.DropTableSystem.Managers;

public static class DropTableTemplateManager
{
    internal static Dictionary<string, DropTableTemplate> Templates { get; set; } = new();

    static DropTableTemplateManager()
    {
        LifecycleManager.SubscribeToWorldInit(() =>
        {
            Templates = new();
        });
    }

    public static void SetTemplate(string prefabName, DropTableTemplate template) =>
        Templates[prefabName] = template;

    public static List<DropTableTemplate> GetTemplates() => Templates.Values.ToList();
         
    public static DropTableTemplate GetTemplate(string prefabName)
    {
        if (Templates.TryGetValue(prefabName, out var template))
        {
            return template;
        }

        return null;
    }

    public static bool TryGetTemplate(string prefabName, out DropTableTemplate template) =>
        Templates.TryGetValue(prefabName, out template);

    public static bool TryGetTemplate(string prefabName, int id, out DropTableDropTemplate dropTemplate)
    {
        if (Templates.TryGetValue(prefabName, out var template))
        {
            return template.Drops.TryGetValue(id, out dropTemplate);
        }

        dropTemplate = null;
        return false;
    }

    internal static void ResetTemplates(IEnumerable<DropTableTemplate> templates)
    {
        Log.Development?.Log("Resetting DropTable templates");

        Templates = templates.ToDictionary(x => x.PrefabName);

#if !TEST
        // Reset currently loaded creatures, so that drop tables can be re-applied.
        foreach (var instance in DropTableManager.DropTableInstances.Values)
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
    ///     Scans all prefabs and returns the expected drop tables after <see cref="DropTableTemplate"/>s
    ///     (if any) have been applied.
    /// </para>
    /// <para>
    ///     This will include default drops if no templates existed.
    /// </para> 
    /// </summary>
    /// <exception cref="InvalidOperationException">If accessed prior to ZnetScene being instantiated.</exception>
    public static Dictionary<string, ExpectedDropTable> GetAllExpectedDropTables()
    {
        return DropExpectationService.AllExpectedDrops();
    }

    /// <summary>
    /// <para>
    ///     Returns the expected drop tables after <see cref="DropTableTemplate"/> 
    ///     (if any) has been applied.
    /// </para>
    /// <para>
    ///     This show default drops if no templates existed for prefab.
    /// </para>
    /// </summary>
    /// <exception cref="InvalidOperationException">If accessed prior to ZnetScene being instantiated.</exception>
    public static bool TryGetExpectedDrops(string prefab, out ExpectedDropTable expectedDropTable) =>
        DropExpectationService.TryGetExpectedDrops(prefab, out expectedDropTable);
}
