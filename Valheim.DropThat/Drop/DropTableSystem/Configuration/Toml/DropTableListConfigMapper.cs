using System;
using System.Collections.Generic;
using ThatCore.Config.Toml.Mapping;
using ThatCore.Config.Toml.Schema;
using ThatCore.Config;
using ThatCore.Config.Toml;

namespace DropThat.Drop.DropTableSystem.Configuration.Toml;

internal class DropTableListConfigMapper
{
    private TomlSchemaBuilder Builder { get; set; }

    private ITomlSchemaNodeBuilder ListNode { get; set; }
    private ITomlSchemaNodeBuilder DropNode { get; set; }

    private ITomlNamedSchemaLayerBuilder ModLayer { get; set; }
    private Dictionary<string, ITomlSchemaNodeBuilder> ModNodes { get; set; } = new();

    private MappingLayer<DropTableSystemConfiguration, object, DropTableListBuilder> ListMappingLayer { get; set; }
    private MappingLayer<DropTableSystemConfiguration, object, DropTableDropBuilder> DropMappingLayer { get; set; }
    private Dictionary<string, MappingLayer<DropTableSystemConfiguration, object, DropTableDropBuilder>> ModMappingLayers { get; } = new();

    public DropTableListConfigMapper()
    {
        Builder = new();

        ListNode = Builder.SetLayerAsCollection().GetNode();
        DropNode = ListNode.SetNextLayerAsCollection().GetNode();
        ModLayer = DropNode.SetNextLayerAsNamed();

        ListMappingLayer = new(
            TomlPathSegmentType.Collection,
            _ => null, // Lists just won't ever map back to files
            ListNode);

        DropMappingLayer = new(
            TomlPathSegmentType.Collection,
            _ => null, // Lists just won't ever map back to files
            DropNode);
    }

    public ITomlSchemaLayer BuildSchema() => Builder.Build();

    public ConfigToObjectMapper<DropTableSystemConfiguration> CreateMapper(DropTableSystemConfiguration configSystem)
    {
        List<IMappingInstantiationForParent<DropTableDropBuilder>> modLayerMappings = new();

        foreach (var entry in ModMappingLayers)
        {
            MappingInstantiationForParent<DropTableDropBuilder, DropTableDropBuilder> mapping = new()
            {
                SubPath = new() { TomlPath.Create(TomlPathSegmentType.Named, entry.Key) },
                Instantiation = new()
                {
                    Instantiation = (builder, config) => builder,
                    InstanceActions = new() { entry.Value.BuildMapping() }
                }
            };

            modLayerMappings.Add(mapping);
        }

        MappingInstantiationForParent<DropTableListBuilder, DropTableDropBuilder> dropLayerMapping = new()
        {
            SubPath = new() { TomlPath.Create(TomlPathSegmentType.Collection) },
            Instantiation = new()
            {
                Instantiation = (builder, config) => builder.GetDrop(uint.Parse(config.PathSegment.Name)),
                InstanceActions = new() { DropMappingLayer.BuildMapping() }
            },
            SubInstantiations = modLayerMappings
        };

        MappingInstantiationForParent<DropTableSystemConfiguration, DropTableListBuilder> listLayerMappings = new()
        {
            SubPath = new() { TomlPath.Create(TomlPathSegmentType.Collection) },
            Instantiation = new()
            {
                Instantiation = (system, config) => system.GetListBuilder(config.PathSegment.Name),
                InstanceActions = new() { ListMappingLayer.BuildMapping() }
            },
            SubInstantiations = new() { dropLayerMapping }
        };

        return new ConfigToObjectMapper<DropTableSystemConfiguration>()
        {
            Path = new(),
            Instantiation = new() { Instantiation = (_) => configSystem },
            SubInstantiations = new() { listLayerMappings }
        };
    }

    public IOptionBuilder<DropTableListBuilder, object> AddListOption() => ListMappingLayer.AddOption();

    public IOptionBuilder<DropTableDropBuilder, object> AddDropOption() => DropMappingLayer.AddOption();

    public IOptionBuilder<DropTableDropBuilder, object> AddModOption(string modName)
    {
        var modNode = ModNodes[modName] = ModLayer.AddNode(modName);

        if (!ModMappingLayers.TryGetValue(modName, out var modMappinglayer))
        {
            modMappinglayer = ModMappingLayers[modName] = new(
                TomlPathSegmentType.Named,
                x => modName,
                modNode);
        }

        return modMappinglayer.AddOption();
    }

    public DropTableListConfigMapper AddModRequirement(string modName, Func<bool> requirement)
    {
        if (!ModMappingLayers.TryGetValue(modName, out var modMappinglayer))
        {
            var modNode = ModNodes[modName] = ModLayer.AddNode(modName);

            modMappinglayer = ModMappingLayers[modName] = new(
                TomlPathSegmentType.Named,
                x => modName,
                modNode);
        }

        modMappinglayer.AddLayerRequirement(requirement);

        return this;
    }
}
