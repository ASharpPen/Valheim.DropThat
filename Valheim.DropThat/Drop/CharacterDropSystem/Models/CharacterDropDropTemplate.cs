using System.Collections.Generic;
using System.ComponentModel;
using DropThat.Drop.CharacterDropSystem.Conditions;
using DropThat.Drop.Options;
using ThatCore.Models;

namespace DropThat.Drop.CharacterDropSystem.Models;

public class CharacterDropDropTemplate
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

    // TODO: Reconsider if the default settings should actually be optional.
    public Optional<int?> AmountMin { get; set; }

    public Optional<int?> AmountMax { get; set; }

    public Optional<float?> ChanceToDrop { get; set; }

    public Optional<bool?> DropOnePerPlayer { get; set; }

    public Optional<bool?> ScaleByLevel { get; set; }

    public Optional<bool?> AutoStack { get; set; }

    public Optional<int?> AmountLimit { get; set; }
}
