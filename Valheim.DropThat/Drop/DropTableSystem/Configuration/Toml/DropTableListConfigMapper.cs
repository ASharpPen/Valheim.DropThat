using System;
using System.Collections.Generic;
using ThatCore.Config.Toml.Mapping;
using ThatCore.Config.Toml.Schema;
using ThatCore.Config;
using ThatCore.Config.Toml;

namespace DropThat.Drop.DropTableSystem.Configuration.Toml;

// TODO: More stuff to clean up in here for sure.
// We don't really need the full MappingLayer, as lists are a bunch
// of hacked together stuff anyway. We just need to have the schemas
// so that we can load the config files and merge things together in
// the configuration file manager.
internal sealed class DropTableListConfigMapper
{
    private TomlSchemaBuilder Builder { get; set; }

    private ITomlSchemaNodeBuilder ListNode { get; set; }
    private ITomlSchemaNodeBuilder DropNode { get; set; }

    private ITomlNamedSchemaLayerBuilder ModLayer { get; set; }
    private Dictionary<string, ITomlSchemaNodeBuilder> ModNodes { get; set; } = new();

    private MappingLayer<DropTableSystemConfiguration, object, DropTableDropBuilder> DropMappingLayer { get; set; }
    private Dictionary<string, MappingLayer<DropTableSystemConfiguration, object, DropTableDropBuilder>> ModMappingLayers { get; } = new();

    public DropTableListConfigMapper()
    {
        Builder = new();

        ListNode = Builder.SetLayerAsCollection().GetNode();
        DropNode = ListNode.SetNextLayerAsCollection().GetNode();
        ModLayer = DropNode.SetNextLayerAsNamed();

        DropMappingLayer = new(
            TomlPathSegmentType.Collection,
            _ => null, // Lists just won't ever map back to files
            DropNode);
    }

    public ITomlSchemaLayer BuildSchema() => Builder.Build();

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
