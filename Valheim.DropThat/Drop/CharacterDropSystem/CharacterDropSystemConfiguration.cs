using System;
using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Configuration;
using DropThat.Drop.CharacterDropSystem.Managers;
using ThatCore.Logging;

namespace DropThat.Drop.CharacterDropSystem;

internal class CharacterDropSystemConfiguration : IDropSystemConfig
{
    private Dictionary<string, CharacterDropBuilder> _builders = new();

    private bool finalized = false;

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

        var templates = _builders.Select(x => x.Value.Build());

        CharacterDropTemplateManager.ResetTemplates(templates);
    }
}
