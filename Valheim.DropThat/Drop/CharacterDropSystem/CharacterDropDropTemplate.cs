using System.Collections.Generic;
using System.ComponentModel;
using DropThat.Drop.CharacterDropSystem.Conditions;
using DropThat.Drop.Options;

namespace DropThat.Drop.CharacterDropSystem;

public class CharacterDropDropTemplate
    : IHaveItemModifiers
{
    public List<ICondition> Conditions { get; set; } = new();

    public List<IItemModifier> ItemModifiers { get; set; } = new();

    public string PrefabName { get; set; }

    [DefaultValue(true)]
    public bool Enabled { get; set; } = true;

    [DefaultValue(true)]
    public bool TemplateEnabled { get; set; } = true;

    public int? AmountMin { get; set; }

    public int? AmountMax { get; set; }

    public float? ChanceToDrop { get; set; }

    public bool? DropOnePerPlayer { get; set; }

    public bool? ScaleByLevel { get; set; }

    public bool? AutoStack { get; set; }

    public int? AmountLimit { get; set; }
}
