using System;
using Valheim.DropThat.Core.Configuration;

namespace Valheim.DropThat.Configuration.ConfigTypes
{
    [Serializable]
    public class DropTableListConfigurationFile : ConfigWithSubsections<DropTableListConfiguration>, IConfigFile
    {
        protected override DropTableListConfiguration InstantiateSubsection(string subsectionName)
        {
            return new DropTableListConfiguration();
        }
    }

    [Serializable]
    public class DropTableListConfiguration : ConfigWithSubsections<DropTableItemConfiguration>
    {
        protected override DropTableItemConfiguration InstantiateSubsection(string subsectionName)
        {
            return new DropTableItemConfiguration();
        }
    }
}
