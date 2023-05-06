using DropThat.Drop.CharacterDropSystem.Conditions;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Drop.Options;
using ThatCore.Extensions;
using ThatCore.Models;

namespace DropThat.Drop.CharacterDropSystem.Configuration;

internal class CharacterDropDropBuilder
    : IHaveDropConditions
    , IHaveItemModifiers
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
            Conditions = Conditions.Clone(),
            ItemModifiers = ItemModifiers.Clone(),
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

        return template;
    }

    public void Configure(CharacterDropDropBuilder from)
    {
        from.Conditions.ForEach(Conditions.Set);
        from.ItemModifiers.ForEach(ItemModifiers.Set);

        from.PrefabName.DoIfSet(x => PrefabName = x);
        from.Enabled.DoIfSet(x => Enabled = x);
        from.TemplateEnabled.DoIfSet(x => TemplateEnabled = x);
        from.AmountMin.DoIfSet(x => AmountMin = x);
        from.AmountMax.DoIfSet(x => AmountMax = x);
        from.ChanceToDrop.DoIfSet(x => ChanceToDrop = x);
        from.DropOnePerPlayer.DoIfSet(x => DropOnePerPlayer = x);
        from.ScaleByLevel.DoIfSet(x => ScaleByLevel = x);
        from.AutoStack.DoIfSet(x => AutoStack = x);
        from.AmountLimit.DoIfSet(x => AmountLimit = x);
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
}
