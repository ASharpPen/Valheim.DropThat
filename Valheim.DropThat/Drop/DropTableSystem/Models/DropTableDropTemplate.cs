using System.Collections.Generic;
using System.ComponentModel;
using DropThat.Drop.DropTableSystem.Conditions;
using DropThat.Drop.Options;

namespace DropThat.Drop.DropTableSystem.Models;

public sealed class DropTableDropTemplate
    : IHaveItemModifiers
    , IHaveDropConditions
{
    public ICollection<IDropCondition> Conditions { get; set; } = [];

    public ICollection<IItemModifier> ItemModifiers { get; set; } = [];

    public int Id { get; set; }

    public string PrefabName { get; set; }

    [DefaultValue(true)]
    public bool? Enabled { get; set; } = true;

    [DefaultValue(true)]
    public bool? TemplateEnabled { get; set; } = true;

    public int? AmountMin { get; set; }

    public int? AmountMax { get; set; }

    public float? Weight { get; set; }

    public bool? DisableResourceModifierScaling { get; set; }
}
