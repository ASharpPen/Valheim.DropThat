using System.Collections.Generic;
using DropThat.Configuration.ConfigTypes;
using DropThat.Drop.DropTableSystem.Conditions;
using DropThat.Drop.DropTableSystem.Modifiers;

namespace DropThat.Drop.DropTableSystem;

public class DropTemplate
{
    public DropTableEntityConfiguration EntityConfig { get; set; }

    public DropTableItemConfiguration Config { get; set; }

    public List<IDropTableCondition> Conditions { get; set; } = new();

    public List<IDropTableModifier> Modifiers { get; set; } = new();

    public DropTable.DropData Drop { get; set; }
}