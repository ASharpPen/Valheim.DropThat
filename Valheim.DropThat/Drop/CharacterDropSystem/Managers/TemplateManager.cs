using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Configuration;
using DropThat.Reset;

namespace DropThat.Drop.CharacterDropSystem.Managers;

public static class DropTemplateManager
{
    internal static Dictionary<string, CharacterDropMobTemplate> Templates { get; set; } = new();

    static DropTemplateManager()
    {
        StateResetter.Subscribe(() =>
        {
            Templates = new();
        });
    }

    public static List<(string prefab, CharacterDropMobTemplate)> GetTemplates() =>
        Templates
        .Select(x => (x.Key, x.Value))
        .ToList();

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
}
