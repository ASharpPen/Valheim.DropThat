using System;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;
using Valheim.DropThat.Core.Network;

namespace Valheim.DropThat.Configuration.Multiplayer
{
    [Serializable]
    internal class ConfigPackage : CompressedPackage
    {
        public GeneralConfiguration GeneralConfig;

        public CharacterDropConfiguration CharacterDropConfigs;

        public CharacterDropListConfigurationFile CharacterDropLists;

        public DropTableConfiguration DropTableConfigs;

        public DropTableListConfigurationFile DropTableLists;

        protected override void BeforePack()
        {
            GeneralConfig = ConfigurationManager.GeneralConfig;

            CharacterDropConfigs = ConfigurationManager.CharacterDropConfigs;
            CharacterDropLists = ConfigurationManager.CharacterDropLists;

            DropTableConfigs = ConfigurationManager.DropTableConfigs;
            DropTableLists = ConfigurationManager.DropTableLists;

            Log.LogDebug("Packaged general config");
            Log.LogDebug($"Packaged creature configurations: {ConfigurationManager.CharacterDropConfigs?.Subsections?.Count ?? 0}");
            Log.LogDebug($"Packaged creature drop lists: {ConfigurationManager.CharacterDropLists?.Subsections?.Count ?? 0}");
            Log.LogDebug($"Packaged drop table configurations: {ConfigurationManager.DropTableConfigs?.Subsections?.Count ?? 0}");
            Log.LogDebug($"Packaged drop table lists: {ConfigurationManager.DropTableLists?.Subsections?.Count ?? 0}");

            Log.LogTrace("Serializing configs.");
        }

        protected override void AfterUnpack(object responseObject)
        {
            if (responseObject is ConfigPackage configPackage)
            {
                Log.LogDebug("Received and deserialized config package");

                Log.LogTrace("Unpackaging configs.");

                ConfigurationManager.GeneralConfig = configPackage.GeneralConfig;
                ConfigurationManager.CharacterDropConfigs = configPackage.CharacterDropConfigs;
                ConfigurationManager.CharacterDropLists = configPackage.CharacterDropLists;
                ConfigurationManager.DropTableConfigs = configPackage.DropTableConfigs;
                ConfigurationManager.DropTableLists = configPackage.DropTableLists;

                Log.LogDebug("Unpacked general config");
                Log.LogDebug($"Unpacked creature configurations: {ConfigurationManager.CharacterDropConfigs?.Subsections?.Count ?? 0}");
                Log.LogDebug($"Unpacked creature drop lists: {ConfigurationManager.CharacterDropLists?.Subsections?.Count ?? 0}");
                Log.LogDebug($"Unpacked drop table configurations: {ConfigurationManager.DropTableConfigs?.Subsections?.Count ?? 0}");
                Log.LogDebug($"Unpacked drop table lists: {ConfigurationManager.DropTableLists?.Subsections?.Count ?? 0}");

                Log.LogInfo("Successfully unpacked configs.");
            }
            else
            {
                Log.LogWarning("Received bad config package. Unable to load.");
            }
        }
    }
}
