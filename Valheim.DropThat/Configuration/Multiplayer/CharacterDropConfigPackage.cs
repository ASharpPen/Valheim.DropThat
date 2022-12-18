using System;
using System.Linq;
using DropThat.Configuration.ConfigTypes;
using DropThat.Core;
using DropThat.Core.Network;

namespace DropThat.Configuration.Multiplayer;

[Serializable]
internal class CharacterDropConfigPackage : Dto
{
    public CharacterDropConfiguration CharacterDropConfigs;
    public CharacterDropListConfigurationFile CharacterDropLists;

    public override void BeforePack()
    {
        CharacterDropConfigs = ConfigurationManager.CharacterDropConfigs;
        CharacterDropLists = ConfigurationManager.CharacterDropLists;

        Log.LogDebug($"Packaged CharacterDrop configurations: " +
            $"{ConfigurationManager.CharacterDropConfigs?.Subsections?.Count ?? 0} creatures " +
            $"and {ConfigurationManager.CharacterDropConfigs?.Subsections?.Values.Sum(x => x.Subsections.Count) ?? 0} drops");
        Log.LogDebug($"Packaged CharacterDrop lists: " +
            $"{ConfigurationManager.CharacterDropLists?.Subsections?.Count ?? 0} creatures " +
            $"and {ConfigurationManager.CharacterDropLists?.Subsections?.Values.Sum(x => x.Subsections.Count) ?? 0} drops");
    }

    public override void AfterUnpack()
    {
        Log.LogDebug("Received and deserialized CharacterDrop config package");

        ConfigurationManager.CharacterDropConfigs = CharacterDropConfigs;
        ConfigurationManager.CharacterDropLists = CharacterDropLists;

        Log.LogDebug($"Unpacked CharacterDrop configurations: " +
            $"{ConfigurationManager.CharacterDropConfigs?.Subsections?.Count ?? 0} creatures " +
            $"and {ConfigurationManager.CharacterDropConfigs?.Subsections?.Values.Sum(x => x.Subsections.Count) ?? 0} drops");
        Log.LogDebug($"Unpacked CharacterDrop lists: " +
            $"{ConfigurationManager.CharacterDropLists?.Subsections?.Count ?? 0} creatures " +
            $"and {ConfigurationManager.CharacterDropLists?.Subsections?.Values.Sum(x => x.Subsections.Count) ?? 0} drops");

        Log.LogInfo("Successfully unpacked CharacterDrop configs.");
    }
}
