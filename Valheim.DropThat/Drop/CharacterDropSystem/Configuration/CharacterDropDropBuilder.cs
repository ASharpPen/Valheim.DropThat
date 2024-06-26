﻿using System.Linq;
using DropThat.Drop.CharacterDropSystem.Conditions;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Drop.Options;
using ThatCore.Models;

namespace DropThat.Drop.CharacterDropSystem.Configuration;

internal sealed class CharacterDropDropBuilder
{
    public CharacterDropDropBuilder(
        uint Id,
        IHaveDropBuilders parent)
    {
        this.Id = Id;
        Parent = parent;
    }

    public uint Id { get; }

    public IHaveDropBuilders Parent { get; }

    public CharacterDropDropTemplate Build()
    {
        CharacterDropDropTemplate template = new()
        {
            Id = (int)Id,
            Conditions = Conditions
                .Where(x => !x.IsPointless())
                .ToList(),
            ItemModifiers = ItemModifiers
                .Where(x => !x.IsPointless())
                .ToList(),
        };

        PrefabName.DoIfSet(x => template.PrefabName = x);
        Enabled.DoIfSet(x => template.Enabled = x);
        TemplateEnabled.DoIfSet(x => template.TemplateEnabled = x);
        AmountMin.DoIfSet(x => template.AmountMin = x);
        AmountMax.DoIfSet(x => template.AmountMax = x);
        ChanceToDrop.DoIfSet(x => template.ChanceToDrop = x);
        DropOnePerPlayer.DoIfSet(x => template.DropOnePerPlayer = x);
        ScaleByLevel.DoIfSet(x => template.ScaleByLevel = x);
        AutoStack.DoIfSet(x => template.AutoStack = x);
        AmountLimit.DoIfSet(x => template.AmountLimit = x);
        DisableResourceModifierScaling.DoIfSet(x => template.DisableResourceModifierScaling = x);

        return template;
    }

    public TypeSet<IDropCondition> Conditions { get; } = new();

    public TypeSet<IItemModifier> ItemModifiers { get; } = new();

    public Optional<string> PrefabName { get; set; }

    public Optional<bool?> Enabled { get; set; }

    public Optional<bool?> TemplateEnabled { get; set; }

    public Optional<int?> AmountMin { get; set; }

    public Optional<int?> AmountMax { get; set; }

    public Optional<float?> ChanceToDrop { get; set; }

    public Optional<bool?> DropOnePerPlayer { get; set; }

    public Optional<bool?> ScaleByLevel { get; set; }

    public Optional<bool?> AutoStack { get; set; }

    public Optional<int?> AmountLimit { get; set; }

    public Optional<bool?> DisableResourceModifierScaling { get; set; }
}
