using System;
using System.Collections.Generic;
using DropThat.Drop.DropTableSystem.Configuration;
using DropThat.Drop.DropTableSystem.Managers;
using ThatCore.Logging;

namespace DropThat.Drop.DropTableSystem;

internal class DropTableSystemConfiguration : IDropSystemConfig
{
    private Dictionary<string, DropTableBuilder> _builders = new();

    private bool _finalized = false;

    public DropTableBuilder GetBuilder(string name)
    {
        if (_finalized)
        {
            throw new InvalidOperationException("Collection is finalized. Builders cannot be retrieved or modified after build.");
        }

        if (_builders.TryGetValue(name, out DropTableBuilder existing))
        {
            Log.Trace?.Log($"Potentially conflicting configurations for droptable '{name}'.");

            return existing;
        }

        return _builders[name] = new DropTableBuilder(name, this);
    }

    public void Build()
    {
        if (_finalized)
        {
            Log.Warning?.Log("Attempting to build character drop configs that have already been finalized. Ignoring request.");
            return;
        }

        _finalized = true;

        foreach (var builder in _builders)
        {
            var template = builder.Value.Build();
            DropTableTemplateManager.SetTemplate(builder.Key, template);
        }
    }
}
