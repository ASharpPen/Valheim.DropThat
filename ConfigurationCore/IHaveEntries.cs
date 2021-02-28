using System.Collections.Generic;

namespace Valheim.DropThat.ConfigurationCore
{
    public interface IHaveEntries
    {
        Dictionary<string, IConfigurationEntry> Entries { get; set; }
    }
}
