using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Reset;

namespace DropThat.Drop.CharacterDropSystem.Managers;

public static class CharacterDropTemplateManager
{
    internal static Dictionary<string, CharacterDropMobTemplate> Templates { get; set; } = new();

    static CharacterDropTemplateManager()
    {
        StateResetter.Subscribe(() =>
        {
            Templates = new();
        });
    }

    public static void SetTemplate(string prefabName, CharacterDropMobTemplate template) =>
        Templates[prefabName] = template;

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
