using System;
using DropThat.Configuration.ConfigTypes;
using DropThat.Core;
using DropThat.Core.Network;

namespace DropThat.Configuration.Multiplayer;

[Serializable]
internal class DropTablePackage : Dto
{
    public DropTableConfiguration DropTableConfigs;
    public DropTableListConfigurationFile DropTableLists;

    public override void BeforePack()
    {
        DropTableConfigs = ConfigurationManager.DropTableConfigs;
        DropTableLists = ConfigurationManager.DropTableLists;

        Log.LogDebug($"Packaged DropTable configurations: {ConfigurationManager.DropTableConfigs?.Subsections?.Count ?? 0}");
        Log.LogDebug($"Packaged DropTable lists: {ConfigurationManager.DropTableLists?.Subsections?.Count ?? 0}");
    }

    public override void AfterUnpack()
    {
        Log.LogDebug("Received and deserialized DropTable config package");

        ConfigurationManager.DropTableConfigs = DropTableConfigs;
        ConfigurationManager.DropTableLists = DropTableLists;

        Log.LogInfo($"Successfully unpacked DropTable configurations: {ConfigurationManager.DropTableConfigs?.Subsections?.Count ?? 0}");
        Log.LogInfo($"Successfully unpacked DropTable lists: {ConfigurationManager.DropTableLists?.Subsections?.Count ?? 0}");
    }
}
