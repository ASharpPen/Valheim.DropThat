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
