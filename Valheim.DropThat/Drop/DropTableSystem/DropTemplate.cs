using System.Collections.Generic;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Drop.DropTableSystem.Conditions;
using Valheim.DropThat.Drop.DropTableSystem.Modifiers;

namespace Valheim.DropThat.Drop.DropTableSystem
{
    public class DropTemplate
    {
        public DropTableEntityConfiguration EntityConfig { get; set; }

        public DropTableItemConfiguration Config { get; set; }

        public List<IDropTableCondition> Conditions { get; set; } = new();

        public List<IDropTableModifier> Modifiers { get; set; } = new();

        public DropTable.DropData Drop { get; set; }
    }
}