using System;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;
using Valheim.DropThat.Core.Network;

namespace Valheim.DropThat.Configuration.Multiplayer
{
    [Serializable]
    internal class DropTablePackage : CompressedPackage
    {
        public DropTableConfiguration DropTableConfigs;
        public DropTableListConfigurationFile DropTableLists;

        protected override void BeforePack()
        {
            DropTableConfigs = ConfigurationManager.DropTableConfigs;
            DropTableLists = ConfigurationManager.DropTableLists;

            Log.LogDebug($"Packaged DropTable configurations: {ConfigurationManager.DropTableConfigs?.Subsections?.Count ?? 0}");
            Log.LogDebug($"Packaged DropTable lists: {ConfigurationManager.DropTableLists?.Subsections?.Count ?? 0}");
        }

        protected override void AfterUnpack(object obj)
        {
            if (obj is DropTablePackage configPackage)
            {
                Log.LogDebug("Received and deserialized DropTable config package");

                ConfigurationManager.DropTableConfigs = configPackage.DropTableConfigs;
                ConfigurationManager.DropTableLists = configPackage.DropTableLists;

                Log.LogDebug($"Unpacked DropTable configurations: {ConfigurationManager.DropTableConfigs?.Subsections?.Count ?? 0}");
                Log.LogDebug($"Unpacked DropTable lists: {ConfigurationManager.DropTableLists?.Subsections?.Count ?? 0}");

                Log.LogInfo("Successfully unpacked DropTable configs.");
            }
            else
            {
                Log.LogWarning("Received bad config package. Unable to load.");
            }
        }
    }
}
