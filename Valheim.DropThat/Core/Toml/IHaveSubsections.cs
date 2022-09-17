using System.Collections.Generic;

namespace DropThat.Core.Toml;

internal interface IHaveSubsections
{
    TomlConfig GetSubsection(string subsectionName);

    List<KeyValuePair<string, TomlConfig>> GetSubsections();
}
