using System;
using ThatCore.Extensions;
using ThatCore.Logging;

namespace DropThat.Drop;

public static class DropSystemConfigManager
{
    public static event Action<IDropSystemConfigCollection> OnConfigure;

    internal static event Action<IDropSystemConfigCollection> OnConfigureLate;

    public static event Action OnConfigsLoaded;

    internal static void ConfigsLoaded()
    {
        Log.Trace?.Log("Running OnConfigsLoaded actions.");
        OnConfigsLoaded.Raise("Error during configs loaded event.");
    }

    internal static void LoadConfigs()
    {
        Log.Trace?.Log("Loading configs.");

        var collection = new DropSystemConfigCollection();

        OnConfigure.Raise(collection);

        OnConfigureLate.Raise(collection);

        collection.Build();

        ConfigsLoaded();
    }
}
