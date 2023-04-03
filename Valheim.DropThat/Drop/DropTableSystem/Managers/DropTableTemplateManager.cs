using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Lifecycle;

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

    public static List<(string prefabName, DropTableTemplate template)> GetTemplates() =>
        Templates
        .Select(x => (x.Key, x.Value))
        .ToList();
         
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
}
