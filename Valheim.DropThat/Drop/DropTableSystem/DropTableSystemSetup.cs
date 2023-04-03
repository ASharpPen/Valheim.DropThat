using DropThat.Configuration;
using DropThat.Drop.CharacterDropSystem.Debug;
using DropThat.Drop.DropTableSystem.Configuration;
using DropThat.Drop.DropTableSystem.Configuration.Toml;
using ThatCore.Lifecycle;

namespace DropThat.Drop.DropTableSystem;

internal static class DropTableSystemSetup
{
    public static void PrepareModule()
    {
        LifecycleManager.OnSinglePlayerInit += LoadConfigs;
        LifecycleManager.OnDedicatedServerInit += LoadConfigs;
    }

    private static void LoadConfigs()
    {
        var configuration = new DropTableSystemConfiguration();

        ConfigurationFileManager.LoadConfigs(configuration);

        configuration.Build();

        if (ConfigurationManager.GeneralConfig?.WriteDropTablesToFiles)
        {
            TemplateWriter.WriteToDiskAsToml();
        }
    }
}
