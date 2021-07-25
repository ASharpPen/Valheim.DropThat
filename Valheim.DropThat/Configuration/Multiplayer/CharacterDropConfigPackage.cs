using System;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;
using Valheim.DropThat.Core.Network;

namespace Valheim.DropThat.Configuration.Multiplayer
{
    [Serializable]
    internal class CharacterDropConfigPackage : CompressedPackage
    {
        public CharacterDropConfiguration CharacterDropConfigs;
        public CharacterDropListConfigurationFile CharacterDropLists;

        protected override void BeforePack()
        {
            CharacterDropConfigs = ConfigurationManager.CharacterDropConfigs;
            CharacterDropLists = ConfigurationManager.CharacterDropLists;

            Log.LogDebug($"Packaged CharacterDrop configurations: {ConfigurationManager.CharacterDropConfigs?.Subsections?.Count ?? 0}");
            Log.LogDebug($"Packaged CharacterDrop lists: {ConfigurationManager.CharacterDropLists?.Subsections?.Count ?? 0}");
        }

        protected override void AfterUnpack(object obj)
        {
            if (obj is CharacterDropConfigPackage configPackage)
            {
                Log.LogDebug("Received and deserialized CharacterDrop config package");

                ConfigurationManager.CharacterDropConfigs = configPackage.CharacterDropConfigs;
                ConfigurationManager.CharacterDropLists = configPackage.CharacterDropLists;

                Log.LogDebug($"Unpacked CharacterDrop configurations: {ConfigurationManager.CharacterDropConfigs?.Subsections?.Count ?? 0}");
                Log.LogDebug($"Unpacked CharacterDrop lists: {ConfigurationManager.CharacterDropLists?.Subsections?.Count ?? 0}");

                Log.LogInfo("Successfully unpacked CharacterDrop configs.");
            }
            else
            {
                Log.LogWarning("Received bad config package. Unable to load.");
            }
        }
    }
}
