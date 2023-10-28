using DropThat.Drop.DropTableSystem.Conditions;
using DropThat.Drop.DropTableSystem.Models;
using DropThat.Drop.Options;
using ThatCore.Extensions;
using ThatCore.Models;

namespace DropThat.Drop.DropTableSystem.Configuration;

internal class DropTableDropBuilder
    : IHaveDropConditions
    , IHaveItemModifiers
{
    // Builder settings

    public uint Id { get; }

    public IHaveDropBuilders Parent { get; }

    // Drop settings

    public TypeSet<IDropCondition> Conditions { get; } = new();

    public TypeSet<IItemModifier> ItemModifiers { get; } = new();

    public Optional<string> PrefabName { get; set; }

    public Optional<bool?> Enabled { get; set; }

    public Optional<bool?> TemplateEnabled { get; set; }

    public Optional<int?> AmountMin { get; set; }

    public Optional<int?> AmountMax { get; set; }

    public Optional<float?> Weight { get; set; }

    public Optional<bool?> DisableResourceModifierScaling { get; set; }

    public DropTableDropBuilder(
        uint id,
        IHaveDropBuilders parent)
    {
        Id = id;
        Parent = parent;
    }

    public DropTableDropTemplate Build()
    {
        DropTableDropTemplate template = new()
        {
            Id = (int)Id,
            Conditions = Conditions.Clone(),
            ItemModifiers = ItemModifiers.Clone(),
        };

        PrefabName.DoIfSet(x => template.PrefabName = x);
        Enabled.DoIfSet(x => template.Enabled = x);
        TemplateEnabled.DoIfSet(x => template.TemplateEnabled = x);
        AmountMin.DoIfSet(x => template.AmountMin = x);
        AmountMax.DoIfSet(x => template.AmountMax = x);
        Weight.DoIfSet(x => template.Weight = x);
        DisableResourceModifierScaling.DoIfSet(x => template.DisableResourceModifierScaling = x);

        return template;
    }
}
