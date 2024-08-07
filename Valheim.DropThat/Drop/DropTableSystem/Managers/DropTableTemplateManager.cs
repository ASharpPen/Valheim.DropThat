﻿using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Models;
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
        if (prefabName is not null &&
            Templates.TryGetValue(prefabName, out var template))
        {
            return template;
        }

        return null;
    }

    public static bool TryGetTemplate(string prefabName, out DropTableTemplate template)
    {
        if (prefabName is not null)
        {
            return Templates.TryGetValue(prefabName, out template);
        }
        template = null;
        return false;
    }
        

    public static bool TryGetTemplate(string prefabName, int id, out DropTableDropTemplate dropTemplate)
    {
        if (prefabName is not null &&
            Templates.TryGetValue(prefabName, out var template))
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
        foreach (var instance in DropTableSessionManager.DropTableInstances.Values)
        {
            if (instance.IsNotNull())
            {
                // Delete gameobject, and leave it to ZnetScene to re-instantiate it.
                // Replicates behaviour of ZnetScene.RemoveObjects removing objects far away.
                if (!instance.TryGetComponent<ZNetView>(out ZNetView znetView))
                {
                    continue;
                }

                ZDO zdo = znetView.GetZDO();

                if (zdo is null)
                {
                    continue;
                }

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
}
