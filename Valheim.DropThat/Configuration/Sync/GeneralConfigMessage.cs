using ThatCore.Logging;
using ThatCore.Network;

namespace DropThat.Configuration.Sync;

internal class GeneralConfigMessage : IMessage
{
    public GeneralConfig GeneralConfig;

    public void Initialize()
    {
        GeneralConfig = GeneralConfigManager.Config;
    }

    public void AfterUnpack()
    {
        GeneralConfigManager.Set(GeneralConfig);

        Log.Debug?.Log("Successfully unpacked general configs.");
    }
}
