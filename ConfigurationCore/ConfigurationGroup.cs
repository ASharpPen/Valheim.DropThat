using System.Collections.Generic;

namespace Valheim.DropThat.ConfigurationCore
{
    public class ConfigurationGroup<TSection> : IHaveEntries where TSection : ConfigurationSection
    {
        public string GroupName { get; set; }

        public Dictionary<string, TSection> Sections { get; set; }

        public Dictionary<string, IConfigurationEntry> Entries { get; set; }

        public ConfigurationGroup()
        {
        }
    }
}
