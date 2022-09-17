using DropThat.Core.Toml;

namespace DropThat.Drop.CharacterDropSystem.Configurations;

internal class CharacterDropListConfigurationFile : TomlConfigWithSubsections<CharacterDropListConfiguration>, ITomlConfigFile
{
    protected override CharacterDropListConfiguration InstantiateSubsection(string subsectionName)
    {
        return new();
    }
}

internal class CharacterDropListConfiguration : TomlConfigWithSubsections<CharacterDropItemConfiguration>
{
    protected override CharacterDropItemConfiguration InstantiateSubsection(string subsectionName)
    {
        return new()
        {
            IsFromNamedList = true
        };
    }
}
