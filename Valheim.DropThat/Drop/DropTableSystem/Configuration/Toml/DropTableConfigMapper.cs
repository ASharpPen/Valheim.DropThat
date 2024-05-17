using System;
using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Config;
using ThatCore.Config.Toml;
using ThatCore.Config.Toml.Mapping;
using ThatCore.Config.Toml.Schema;

namespace DropThat.Drop.DropTableSystem.Configuration.Toml;

internal sealed class DropTableConfigMapper
{
    private TomlSchemaBuilder Builder { get; set; }

    private ITomlSchemaNodeBuilder TableNode { get; set; }
    private ITomlSchemaNodeBuilder DropNode { get; set; }

    private ITomlNamedSchemaLayerBuilder ModLayer { get; set; }
    private Dictionary<string, ITomlSchemaNodeBuilder> ModNodes { get; set; } = new();

    private MappingLayer<DropTableSystemConfiguration, DropTableTemplate, DropTableBuilder> TableMappingLayer { get; set; }
    private MappingLayer<DropTableSystemConfiguration, DropTableDropTemplate, DropTableDropBuilder> DropMappingLayer { get; set; }
    private Dictionary<string, MappingLayer<DropTableSystemConfiguration, DropTableDropTemplate, DropTableDropBuilder>> ModMappingLayers { get; } = new();

    public DropTableConfigMapper()
    {
        Builder = new();

        TableNode = Builder.SetLayerAsCollection().GetNode();
        DropNode = TableNode.SetNextLayerAsCollection().GetNode();
        ModLayer = DropNode.SetNextLayerAsNamed();

        TableMappingLayer = new(
            TomlPathSegmentType.Collection,
            x => x.PrefabName,
            TableNode);

        DropMappingLayer = new(
            TomlPathSegmentType.Collection,
            x => x.Id.ToString(),
            DropNode);
    }

    public ITomlSchemaLayer BuildSchema() => Builder.Build();

    public ConfigToObjectMapper<DropTableSystemConfiguration> CreateMapper(DropTableSystemConfiguration configSystem)
    {
        List<IMappingInstantiationForParent<DropTableDropBuilder>> modLayerMappings = new();

        foreach(var entry in ModMappingLayers)
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

        MappingInstantiationForParent<DropTableBuilder, DropTableDropBuilder> dropLayerMapping = new()
        {
            SubPath = new() { TomlPath.Create(TomlPathSegmentType.Collection) },
            Instantiation = new()
            {
                Instantiation = (builder, config) => builder.GetDrop(uint.Parse(config.PathSegment.Name)),
                InstanceActions = new() { DropMappingLayer.BuildMapping() }
            },
            SubInstantiations = modLayerMappings
        };

        MappingInstantiationForParent<DropTableSystemConfiguration, DropTableBuilder> tableLayerMappings = new()
        {
            SubPath = new() { TomlPath.Create(TomlPathSegmentType.Collection) },
            Instantiation = new()
            {
                Instantiation = (system, config) => system.GetBuilder(config.PathSegment.Name),
                InstanceActions = new() { TableMappingLayer.BuildMapping() }
            },
            SubInstantiations = new() { dropLayerMapping }
        };

        return new ConfigToObjectMapper<DropTableSystemConfiguration>()
        {
            Path = new(),
            Instantiation = new() { Instantiation = (_) => configSystem },
            SubInstantiations = new() { tableLayerMappings }
        };
    }

    public TomlConfig MapToConfigFromTemplates(IEnumerable<DropTableTemplate> templates)
    {
        TomlConfig config = new();

        foreach(var tableTemplate in templates)
        {
            var tableConfig = TableMappingLayer.Execute(tableTemplate, config);

            foreach (var drop in tableTemplate.Drops.OrderBy(x => x.Key))
            {
                var dropConfig = DropMappingLayer.Execute(drop.Value, tableConfig);

                foreach (var mod in ModMappingLayers)
                {
                    mod.Value.Execute(drop.Value, dropConfig);
                }
            }
        }

        return config;
    }

    public IOptionBuilder<DropTableBuilder, DropTableTemplate> AddTableOption() => TableMappingLayer.AddOption();

    public IOptionBuilder<DropTableDropBuilder, DropTableDropTemplate> AddDropOption() => DropMappingLayer.AddOption();

    public IOptionBuilder<DropTableDropBuilder, DropTableDropTemplate> AddModOption(string modName)
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

    public DropTableConfigMapper AddModRequirement(string modName, Func<bool> requirement)
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
