using System;
using Valheim.DropThat.Core.Configuration;

namespace Valheim.DropThat.Configuration.ConfigTypes
{
    [Serializable]
    public class CharacterDropListConfigurationFile : ConfigWithSubsections<CharacterDropListConfiguration>, IConfigFile
    {
        protected override CharacterDropListConfiguration InstantiateSubsection(string subsectionName)
        {
            return new CharacterDropListConfiguration();
        }
    }

    [Serializable]
    public class CharacterDropListConfiguration : ConfigWithSubsections<CharacterDropItemConfiguration>
    {
        protected override CharacterDropItemConfiguration InstantiateSubsection(string subsectionName)
        {
            return new CharacterDropItemConfiguration()
            {
                IsFromNamedList = true,
            };
        }
    }
}
