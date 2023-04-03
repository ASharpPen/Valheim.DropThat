using System;
using System.Collections.Generic;
using System.Linq;
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

    public List<IDropCondition> Conditions { get; } = new();

    public List<IItemModifier> ItemModifiers { get; } = new();

    public Optional<string> PrefabName { get; set; }

    public Optional<bool?> Enabled { get; set; }

    public Optional<bool?> TemplateEnabled { get; set; }

    public Optional<int?> AmountMin { get; set; }

    public Optional<int?> AmountMax { get; set; }

    public Optional<float?> Weight { get; set; }

    public DropTableDropBuilder(
        uint id,
        IHaveDropBuilders parent)
    {
        Id = id;
        Parent = parent;
    }

    public void Configure(DropTableDropBuilder from)
    {
        from.Conditions.ForEach(x => Conditions.AddOrReplaceByType(x));
        from.ItemModifiers.ForEach(x => ItemModifiers.AddOrReplaceByType(x));

        from.PrefabName.AssignIfSet(PrefabName);
        from.Enabled.AssignIfSet(Enabled);
        from.TemplateEnabled.AssignIfSet(TemplateEnabled);
        from.AmountMin.AssignIfSet(AmountMin);
        from.AmountMax.AssignIfSet(AmountMax);
        from.Weight.AssignIfSet(Weight);
    }

    public DropTableDropTemplate Build()
    {
        DropTableDropTemplate template = new()
        {
            Conditions = Conditions.ToList(),
            ItemModifiers = ItemModifiers.ToList(),
        };

        PrefabName.DoIfSet(x => template.PrefabName = x);
        Enabled.DoIfSet(x => template.Enabled = x);
        TemplateEnabled.DoIfSet(x => template.TemplateEnabled = x);
        AmountMin.DoIfSet(x => template.AmountMin = x);
        AmountMax.DoIfSet(x => template.AmountMax = x);
        Weight.DoIfSet(x => template.Weight = x);

        return template;
    }
}
