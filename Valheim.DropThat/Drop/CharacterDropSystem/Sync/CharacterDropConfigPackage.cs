using System;
using DropThat.Core.Network;
using DropThat.Core;
using DropThat.Drop.CharacterDropSystem.Configurations;

namespace DropThat.Drop.CharacterDropSystem.Sync;

[Serializable]
internal class CharacterDropConfigPackage : CompressedPackage
{
    public CharacterDropConfigurationFile CharacterDropConfigs;
    public CharacterDropListConfigurationFile CharacterDropLists;

    protected override void BeforePack()
    {
        CharacterDropConfigs = CharacterDropConfigurationFileManager.CharacterDropConfig;
        CharacterDropLists = CharacterDropConfigurationFileManager.CharacterDropListConfig;

        Log.LogDebug($"Packaged CharacterDrop configurations: {CharacterDropConfigurationFileManager.CharacterDropConfig?.Subsections?.Count ?? 0}");
        Log.LogDebug($"Packaged CharacterDrop lists: {CharacterDropConfigurationFileManager.CharacterDropListConfig?.Subsections?.Count ?? 0}");
    }

    protected override void AfterUnpack(object obj)
    {
        if (obj is CharacterDropConfigPackage configPackage)
        {
            Log.LogDebug("Received and deserialized CharacterDrop config package");

            CharacterDropConfigurationFileManager.CharacterDropConfig = configPackage.CharacterDropConfigs;
            CharacterDropConfigurationFileManager.CharacterDropListConfig = configPackage.CharacterDropLists;

            Log.LogDebug($"Unpacked CharacterDrop configurations: {CharacterDropConfigurationFileManager.CharacterDropConfig?.Subsections?.Count ?? 0}");
            Log.LogDebug($"Unpacked CharacterDrop lists: {CharacterDropConfigurationFileManager.CharacterDropListConfig?.Subsections?.Count ?? 0}");

            Log.LogInfo("Successfully unpacked CharacterDrop configs.");
        }
        else
        {
            Log.LogWarning("Received bad config package. Unable to load.");
        }
    }
}
