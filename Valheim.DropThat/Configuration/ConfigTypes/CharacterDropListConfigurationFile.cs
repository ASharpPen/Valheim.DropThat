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
    public class CharacterDropListConfiguration : ConfigWithSubsections<DropItemConfiguration>
    {
        protected override DropItemConfiguration InstantiateSubsection(string subsectionName)
        {
            return new DropItemConfiguration();
        }
    }
}
