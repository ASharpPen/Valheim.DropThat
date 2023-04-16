using System;
using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem;
using DropThat.Drop.CharacterDropSystem.Configuration;
using DropThat.Drop.CharacterDropSystem.Managers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThatCore.Config.Toml;
using ThatCore.Config.Toml.Mapping;
using ThatCore.Config.Toml.Schema;

namespace Valheim.DropThat.Tests.Drop.CharacterDropSystem.Configuration;

[TestClass]
public class MappingTests
{
    private TomlSchemaBuilder _schemaBuilder;
    private TomlSchemaBuilder _listSchemaBuilder;

    private ITomlSchemaLayer _schema;
    private ITomlSchemaLayer _listSchema;

    [TestInitialize]
    public void Initialize()
    {
        if (_schema is null)
        {
            _schemaBuilder = CharacterDropSchemaGenerator.GenerateCfgSchema();
            _schema = _schemaBuilder.Build();
        }

        if (_listSchema is null)
        {
            _listSchemaBuilder = CharacterDropListSchemaGenerator.GenerateCfgSchema();
            _listSchema = _listSchemaBuilder.Build();
        }
    }

    [TestMethod]
    public void CanLoadFile()
    {
        // Arrange
        ConfigurationFileManager.Clear();

        CharacterDropSystemConfiguration config = new();
        ConfigToObjectMapper<CharacterDropSystemConfiguration> configMapper = CharacterDropSchemaGenerator.GenerateConfigLoader(config);

        // Act
        var configFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestMapping, _schema);

        // Assert
        configFile.Should().NotBeNull();
    }

    [TestMethod]
    public void CanExecuteMapping()
    {
        // Arrange
        ConfigurationFileManager.Clear();

        CharacterDropSystemConfiguration config = new();
        ConfigToObjectMapper<CharacterDropSystemConfiguration> configMapper = CharacterDropSchemaGenerator.GenerateConfigLoader(config);

        TomlConfig configFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestMapping, _schema);

        // Act
        Func<CharacterDropSystemConfiguration> act = () => configMapper.Execute(configFile);

        // Assert
        act.Should().NotThrow();
    }

    [TestMethod]
    public void CanBuild()
    {
        // Arrange
        ConfigurationFileManager.Clear();

        CharacterDropSystemConfiguration config = new();
        ConfigToObjectMapper<CharacterDropSystemConfiguration> configMapper = CharacterDropSchemaGenerator.GenerateConfigLoader(config);

        TomlConfig configFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestMapping, _schema);

        // Act
        configMapper.Execute(configFile);
        config.Build();

        // Assert
        bool isLoaded = CharacterDropTemplateManager.TryGetTemplate("Boar", out var mobTemplate);

        isLoaded.Should().BeTrue();
        mobTemplate.Drops.Should().HaveCountGreaterThan(0);
    }

    [TestMethod]
    public void CanMapDrops()
    {
        // Arrange
        ConfigurationFileManager.Clear();

        CharacterDropSystemConfiguration config = new();
        ConfigToObjectMapper<CharacterDropSystemConfiguration> configMapper = CharacterDropSchemaGenerator.GenerateConfigLoader(config);

        TomlConfig configFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestMapping, _schema);

        // Act
        configMapper.Execute(configFile);
        config.Build();
        CharacterDropTemplateManager.TryGetTemplate("Boar", out var mobTemplate);

        // Assert
        var dropTemplate = mobTemplate.Drops.First().Value;
        dropTemplate.PrefabName.Should().Be("Guck");
    }
}
