using System;
using System.Collections.Generic;

namespace Valheim.DropThat.ConfigurationCore
{
    [Serializable]
    public abstract class ConfigurationSection : IHaveEntries
    {
        public string SectionName { get; set; }

        public Dictionary<string, IConfigurationEntry> Entries { get; set; }
    }
}
