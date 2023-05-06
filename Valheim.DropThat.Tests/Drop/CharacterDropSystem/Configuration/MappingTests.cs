using System;
using System.Linq;
using DropThat.Drop.CharacterDropSystem;
using DropThat.Drop.CharacterDropSystem.Configuration;
using DropThat.Drop.CharacterDropSystem.Configuration.Toml;
using DropThat.Drop.CharacterDropSystem.Managers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThatCore.Config.Toml;
using ThatCore.Config.Toml.Schema;

namespace Valheim.DropThat.Tests.Drop.CharacterDropSystem.Configuration;

[TestClass]
public class MappingTests
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
        var schema = _mapper.BuildSchema();

        // Act
        var configFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestMapping, schema);

        // Assert
        configFile.Should().NotBeNull();
    }

    [TestMethod]
    public void CanExecuteMapping()
    {
        // Arrange
        var schema = _mapper.BuildSchema();
        var configMapper = _mapper.CreateMapper(new());

        TomlConfig configFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestMapping, schema);

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

        var schema = _mapper.BuildSchema();
        var configMapper = _mapper.CreateMapper(config);

        TomlConfig configFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestMapping, schema);

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
        CharacterDropTemplateManager.Templates.Clear();

        var config = new CharacterDropSystemConfiguration();

        var schema = _mapper.BuildSchema();
        var configMapper = _mapper.CreateMapper(config);

        TomlConfig configFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.CharacterDrop.TestMapping, schema);

        // Act
        configMapper.Execute(configFile);
        config.Build();
        CharacterDropTemplateManager.TryGetTemplate("Boar", out var mobTemplate);

        // Assert
        mobTemplate.Drops[0].PrefabName.Should().Be("Guck");
    }

    [TestMethod]
    public void CanPrintSchema()
    {
        // Arrange
        var schema = _mapper.BuildSchema();

        // Act
        var result = TomlSchemaWriter.WriteToString(schema, new() { AddComments = true });

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
    }
}
