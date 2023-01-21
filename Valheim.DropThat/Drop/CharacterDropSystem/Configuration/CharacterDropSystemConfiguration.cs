using System;
using System.Collections.Generic;
using DropThat.Drop.CharacterDropSystem.Managers;
using ThatCore.Logging;

namespace DropThat.Drop.CharacterDropSystem.Configuration;

internal class CharacterDropSystemConfiguration
{
    private Dictionary<string, CharacterDropListBuilder> _listBuilders = new();

    private Dictionary<string, CharacterDropBuilder> _builders = new();

    private bool finalized = false;

    public CharacterDropListBuilder GetListBuilder(string name)
    {
        if (_listBuilders.TryGetValue(name, out var existing))
        {
            return existing;
        }

        return _listBuilders[name] = new(name, this);
    }

    public CharacterDropBuilder GetBuilder(string mob)
    {
        if (finalized)
        {
            throw new InvalidOperationException("Collection is finalized. Builders cannot be retrieved or modified after build.");
        }

        if (_builders.TryGetValue(mob, out CharacterDropBuilder existing))
        {
            Log.Trace?.Log($"Potentially conflicting configurations for character drop '{mob}'.");

            return existing;
        }

        return _builders[mob] = new CharacterDropBuilder(mob, this);
    }

    public void Build()
    {
        if (finalized)
        {
            Log.Warning?.Log("Attempting to build character drop configs that have already been finalized. Ignoring request.");
            return;
        }

        finalized = true;

        foreach (var builder in _builders)
        {
            var template = builder.Value.Build();
            TemplateManager.SetTemplate(builder.Key, template);
        }
    }
}
