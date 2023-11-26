using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Managers;
using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Logging;
using ThatCore.Network;

namespace DropThat.Drop.DropTableSystem.Sync;

internal class DropTableConfigMessage : IMessage
{
    public Dictionary<string, DropTableTemplate> Templates { get; set; }

    public DropTableConfigMessage()
    {
    }

    public void Initialize()
    {
        Templates = new(DropTableTemplateManager.Templates);

        Log.Debug?.Log($"Packaged DropTable configurations: " +
            $"{Templates.Values.Sum(x => x.Drops.Count)} drops for {Templates.Count} drop tables");
    }

    public void AfterUnpack()
    {
        Templates ??= new(0);

        DropTableTemplateManager.ResetTemplates(Templates.Values);

        Log.Debug?.Log($"Unpacked DropTable configurations: " +
            $"{Templates.Values.Sum(x => x.Drops.Count)} drops for {Templates.Count} drop tables");
    }
}
