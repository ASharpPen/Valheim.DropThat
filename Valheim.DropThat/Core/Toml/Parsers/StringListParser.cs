using System.Collections.Generic;
using DropThat.Core.Extensions;

namespace DropThat.Core.Toml.Parsers;

internal class StringListParser : ValueParser<List<string>>
{
    protected override void ParseInternal(ITomlConfigEntry<List<string>> entry, TomlLine line)
    {
        entry.Value = line.Value.SplitByComma();
        entry.IsSet = true;
    }
}
