using System;
using ThatCore.Extensions;
using ThatCore.Logging;

namespace DropThat.Drop;

public static class DropSystemConfigManager
{
    public static event Action<IDropSystemConfigCollection> OnConfigure;

    internal static event Action<IDropSystemConfigCollection> OnConfigureLate;

    public static event Action OnConfigsLoaded;

    public static event Action OnReceivedCharacterDropConfigs;

    public static event Action OnReceivedDropTableConfigs;

    internal static void ConfigsLoaded()
    {
        Log.Trace?.Log("Running OnConfigsLoaded actions.");
        OnConfigsLoaded.Raise("Error during configs loaded event.");
    }

    internal static void ReceivedCharacterDropConfigs()
    {
        Log.Trace?.Log("Running OnReceivedCharacterDropConfigs actions.");
        OnReceivedCharacterDropConfigs.Raise("Error during character_drop configs received event.");
    }

    internal static void ReceivedDropTableConfigs()
    {
        Log.Trace?.Log("Running OnReceivedDropTableConfigs actions.");
        OnReceivedDropTableConfigs.Raise("Error during drop_table configs received event.");
    }

    internal static void LoadConfigs()
    {
        Log.Trace?.Log("Loading configs.");

        var collection = new DropSystemConfigCollection();

        OnConfigure.Raise(collection);

        OnConfigureLate.Raise(collection);

        foreach (var dropSystem in collection.GetDropSystemConfigs())
        {
            try
            {
                dropSystem.Build();
            }
            catch (Exception e)
            {
                Log.Error?.Log($"Error during build of drop config {dropSystem?.GetType()?.Name}", e);
            }
        }

        ConfigsLoaded();
    }
}
