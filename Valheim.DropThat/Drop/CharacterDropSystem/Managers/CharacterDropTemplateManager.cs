using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Lifecycle;

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
        Templates[prefabName.ToUpperInvariant()] = template;

    public static List<CharacterDropMobTemplate> GetTemplates() => Templates.Values.ToList();

    public static CharacterDropMobTemplate GetTemplate(string prefabName)
    {
        if (Templates.TryGetValue(prefabName.ToUpperInvariant(), out var template))
        {
            return template;
        }

        return null;
    }

    public static bool TryGetTemplate(string prefabName, out CharacterDropMobTemplate template) =>
        Templates.TryGetValue(prefabName.ToUpperInvariant(), out template);

    public static bool TryGetTemplate(string prefabName, int id, out CharacterDropDropTemplate template)
    {
        if (Templates.TryGetValue(prefabName.ToUpperInvariant(), out var mobTemplate))
        {
            return mobTemplate.Drops.TryGetValue(id, out template);
        }

        template = null;
        return false;
    }
}
