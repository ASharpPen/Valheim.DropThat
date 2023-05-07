using System;
using DropThat.Drop.CharacterDropSystem.Configuration.Toml;
using DropThat.Drop.CharacterDropSystem.Managers;
using DropThat.Drop.CharacterDropSystem;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThatCore.Config.Toml.Schema;
using ThatCore.Config.Toml;
using DropThat.Drop.CharacterDropSystem.Configuration;

namespace Valheim.DropThat.Tests.Drop.CharacterDropSystem.Configuration;

[TestClass]
public class MappingListTests
{
    private CharacterDropConfigMapper _mapper;

    [TestInitialize()]
    public void Initialize()
    {
        _mapper = ConfigurationFileManager.PrepareMappings();
    }

    [TestMethod]
    public void CanLoadFile()
    {
        // Arrange
        var schema = _mapper.BuildListSchema();

        // Act
        var configFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestListMapping, schema);

        // Assert
        configFile.Should().NotBeNull();
    }

    [TestMethod]
    public void CanExecuteMapping()
    {
        // Arrange
        var schema = _mapper.BuildListSchema();
        var configMapper = _mapper.CreateMapper(new());

        TomlConfig configFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestListMapping, schema);

        // Act
        configMapper.Execute(configFile);

        Func<CharacterDropSystemConfiguration> act = () => configMapper.Execute(configFile);

        // Assert
        act.Should().NotThrow();
    }

    [TestMethod]
    public void CanBuild()
    {
        // Arrange
        CharacterDropTemplateManager.Templates.Clear();

        var config = new CharacterDropSystemConfiguration();

        var listSchema = _mapper.BuildListSchema();
        var schema = _mapper.BuildSchema();

        var configMapper = _mapper.CreateMapper(config);

        TomlConfig listConfigFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestListMapping, listSchema);
        TomlConfig mainConfigFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestMapping, schema);

        var configFile = ConfigurationFileManager.MergeLists(listConfigFile, mainConfigFile);

        // Act
        configMapper.Execute(configFile);
        config.Build();

        // Assert
        bool isLoaded = CharacterDropTemplateManager.TryGetTemplate("Boar", out var mobTemplate);

        isLoaded.Should().BeTrue();
        mobTemplate.Drops.Should().HaveCountGreaterThan(1);
    }

    [TestMethod]
    public void CanMapDrops()
    {
        // Arrange
        CharacterDropTemplateManager.Templates.Clear();

        var config = new CharacterDropSystemConfiguration();

        var listSchema = _mapper.BuildListSchema();
        var schema = _mapper.BuildSchema();

        var configMapper = _mapper.CreateMapper(config);

        TomlConfig listConfigFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestListMapping, listSchema);
        TomlConfig mainConfigFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestMapping, schema);

        var configFile = ConfigurationFileManager.MergeLists(listConfigFile, mainConfigFile);

        // Act
        configMapper.Execute(configFile);
        config.Build();
        CharacterDropTemplateManager.TryGetTemplate("Boar", out var mobTemplate);

        // Assert
        mobTemplate.Drops[0].PrefabName.Should().Be("Guck"); // Overridden by the TestMapping file

        mobTemplate.Drops[1].PrefabName.Should().Be("RawMeat"); // Added by TestListMapping, without being overridden.
    }

    [TestMethod]
    public void CanLoadMultipleLists()
    {
        // Arrange
        CharacterDropTemplateManager.Templates.Clear();

        var config = new CharacterDropSystemConfiguration();

        var listSchema = _mapper.BuildListSchema();
        var schema = _mapper.BuildSchema();

        var configMapper = _mapper.CreateMapper(config);

        TomlConfig listConfigFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestListMapping, listSchema);
        TomlConfig listConfigFile2 = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestListMapping2, listSchema);
        TomlConfig mainConfigFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestMapping, schema);

        TomlConfigMerger.Merge(listConfigFile2, listConfigFile);

        var configFile = ConfigurationFileManager.MergeLists(listConfigFile, mainConfigFile);

        // Act
        configMapper.Execute(configFile);
        config.Build();
        CharacterDropTemplateManager.TryGetTemplate("Boar", out var mobTemplate);

        // Assert
        mobTemplate.Drops[0].PrefabName.Should().Be("Guck"); // Overridden by the TestMapping file

        mobTemplate.Drops[1].PrefabName.Should().Be("RawMeat"); // Added by TestListMapping, overridden by , without being overridden.

        mobTemplate.Drops[2].PrefabName.Should().Be("ScrapIron"); //  Added by TestListMapping, overridden by TestListMapping2

        mobTemplate.Drops[3].PrefabName.Should().Be("ScrapIron"); //  Added by TestListMapping2, overridden by , without being overridden.
    }

    [TestMethod]
    public void CanPrintSchema()
    {
        // Arrange
        var schema = _mapper.BuildListSchema();

        // Act
        var result = TomlSchemaWriter.WriteToString(schema, new() { AddComments = true });

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
    }
}
