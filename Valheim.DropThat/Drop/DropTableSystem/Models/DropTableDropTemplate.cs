using System.Collections.Generic;
using System.ComponentModel;
using DropThat.Drop.DropTableSystem.Conditions;
using DropThat.Drop.Options;
using ThatCore.Models;

namespace DropThat.Drop.DropTableSystem.Models;

public class DropTableDropTemplate
    : IHaveItemModifiers
    , IHaveDropConditions
{
    public TypeSet<IDropCondition> Conditions { get; set; } = new();

    public TypeSet<IItemModifier> ItemModifiers { get; set; } = new();

    public int Id { get; set; }

    public Optional<string> PrefabName { get; set; }

    [DefaultValue(true)]
    public bool? Enabled { get; set; } = true;

    [DefaultValue(true)]
    public bool? TemplateEnabled { get; set; } = true;

    public Optional<int?> AmountMin { get; set; }

    public Optional<int?> AmountMax { get; set; }

    public Optional<float?> Weight { get; set; }
}
