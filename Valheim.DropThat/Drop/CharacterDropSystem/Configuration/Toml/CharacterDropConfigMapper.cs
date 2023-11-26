using System;
using System.Collections.Generic;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Config;
using ThatCore.Config.Toml;
using ThatCore.Config.Toml.Mapping;
using ThatCore.Config.Toml.Schema;

namespace DropThat.Drop.CharacterDropSystem.Configuration.Toml;

internal sealed class CharacterDropConfigMapper
{
    private TomlSchemaBuilder Builder { get; set; } = new();

    private ITomlSchemaNodeBuilder MobNode { get; set; }
    private MappingLayer<CharacterDropSystemConfiguration, CharacterDropMobTemplate, CharacterDropBuilder> MobLayer { get; set; }

    private ITomlSchemaNodeBuilder DropNode { get; set; }
    private MappingLayer<CharacterDropBuilder, CharacterDropDropTemplate, CharacterDropDropBuilder> DropLayer { get; set; }

    private ITomlNamedSchemaLayerBuilder DropModLayer { get; set; }
    private Dictionary<string, ITomlSchemaNodeBuilder> DropModNodes { get; } = new();
    private Dictionary<string, MappingLayer<CharacterDropDropBuilder, CharacterDropDropTemplate, CharacterDropDropBuilder>> DropModLayers { get; } = new();

    public CharacterDropConfigMapper()
    {
        MobNode = Builder.SetLayerAsCollection().GetNode();
        DropNode = MobNode.SetNextLayerAsCollection().GetNode();
        DropModLayer = DropNode.SetNextLayerAsNamed();

        MobLayer = new(
            TomlPathSegmentType.Collection,
            x => x.PrefabName,
            MobNode);

        DropLayer = new(
            TomlPathSegmentType.Collection,
            x => x.Id.ToString(),
            DropNode);
    }

    public ITomlSchemaLayer BuildSchema() => Builder.Build();

    public ITomlSchemaLayer BuildListSchema()
    {
        // Just duplicate mob builder
        var schema = Builder.Build();
        // Clear mob settings, as these don't exist for lists.
        schema.AsIndexed().Node.Settings.Clear();

        return schema;
    }

    public ConfigToObjectMapper<CharacterDropSystemConfiguration> CreateMapper(CharacterDropSystemConfiguration configSystem)
    {
        List<IMappingInstantiationForParent<CharacterDropDropBuilder>> modLayerMappings = new();

        foreach (var entry in DropModLayers)
        {
            MappingInstantiationForParent<CharacterDropDropBuilder, CharacterDropDropBuilder> mapping = new()
            {
                SubPath = new()
                {
                    TomlPath.Create(TomlPathSegmentType.Named, entry.Key)
                },
                Instantiation = new()
                {
                    Instantiation = (builder, config) => builder,
                    InstanceActions = new()
                    {
                        entry.Value.BuildMapping(),
                    }
                }
            };

            modLayerMappings.Add(mapping);
        }

        var dropLayerMappings = new MappingInstantiationForParent<CharacterDropBuilder, CharacterDropDropBuilder>()
        {
            SubPath = new()
            {
                TomlPath.Create(TomlPathSegmentType.Collection)
            },
            Instantiation = new()
            {
                Instantiation = (CharacterDropBuilder builder, TomlConfig config) => builder.GetDrop(uint.Parse(config.PathSegment.Name)),
                InstanceActions = new()
                {
                    DropLayer.BuildMapping(),
                }
            },
            SubInstantiations = modLayerMappings,
        };

        var mobLayerMappings = new MappingInstantiationForParent<CharacterDropSystemConfiguration, CharacterDropBuilder>()
        {
            SubPath = new()
            {
                TomlPath.Create(TomlPathSegmentType.Collection)
            },
            Instantiation = new()
            {
                Instantiation = (CharacterDropSystemConfiguration system, TomlConfig config) => system.GetBuilder(config.PathSegment.Name),
                InstanceActions = new()
                {
                    MobLayer.BuildMapping()
                }
            },
            SubInstantiations = new()
            {
                dropLayerMappings
            }
        };

        return new ConfigToObjectMapper<CharacterDropSystemConfiguration>()
        {
            Path = new(),
            Instantiation = new()
            {
                Instantiation = (_) => configSystem,
            },
            SubInstantiations = new()
            {
                mobLayerMappings
            }
        };
    }

    public TomlConfig MapToConfigFromTemplates(IEnumerable<CharacterDropMobTemplate> entries)
    {
        TomlConfig config = new();

        foreach (var mobTemplate in entries)
        {
            var mobConfig = MobLayer.Execute(mobTemplate, config);

            foreach (var drop in mobTemplate.Drops)
            {
                var dropConfig = DropLayer.Execute(drop.Value, mobConfig);

                foreach (var mod in DropModLayers)
                {
                    mod.Value.Execute(drop.Value, dropConfig);
                }
            }
        }

        return config;
    }

    public IOptionBuilder<CharacterDropBuilder, CharacterDropMobTemplate> AddMobSetting() => MobLayer.AddOption();

    public IOptionBuilder<CharacterDropDropBuilder, CharacterDropDropTemplate> AddDropSetting() => DropLayer.AddOption();

    public IOptionBuilder<CharacterDropDropBuilder, CharacterDropDropTemplate> AddModSettings(string modName)
    {
        var modNode = DropModNodes[modName] = DropModLayer.AddNode(modName);

        if (!DropModLayers.TryGetValue(modName, out var modLayer))
        {
            modLayer = DropModLayers[modName] = new(
                TomlPathSegmentType.Named,
                x => modName,
                modNode);
        }

        return modLayer.AddOption();
    }

    public CharacterDropConfigMapper AddModRequirement(string modName, Func<bool> requirement)
    {
        if (!DropModLayers.TryGetValue(modName, out var layer))
        {
            var modNode = DropModNodes[modName] = DropModLayer.AddNode(modName);

            layer = DropModLayers[modName] = new(
                TomlPathSegmentType.Named,
                x => modName,
                modNode);
        }

        layer.AddLayerRequirement(requirement);

        return this;
    }
}
