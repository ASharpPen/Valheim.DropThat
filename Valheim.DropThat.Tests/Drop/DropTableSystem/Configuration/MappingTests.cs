using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropThat.Drop.CharacterDropSystem;
using DropThat.Drop.CharacterDropSystem.Managers;
using DropThat.Drop.DropTableSystem;
using DropThat.Drop.DropTableSystem.Configuration.Toml;
using DropThat.Drop.DropTableSystem.Managers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThatCore.Config.Toml;
using ThatCore.Config.Toml.Schema;

namespace Valheim.DropThat.Tests.Drop.DropTableSystem.Configuration;

[TestClass]
public class MappingTests
{
    private DropTableConfigMapper _mapper;

    [TestInitialize()]
    public void Initialize()
    {
        _mapper = ConfigurationFileManager.RegisterMainMappings();
    }

    [TestMethod]
    public void CanLoadFile()
    {
        // Arrange
        var schema = _mapper.BuildSchema();

        // Act
        var configFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.DropTable.TestMapping, schema);

        // Assert
        configFile.Should().NotBeNull();
    }

    [TestMethod]
    public void CanExecuteMapping()
    {
        // Arrange
        var schema = _mapper.BuildSchema();
        var configMapper = _mapper.CreateMapper(new());

        TomlConfig configFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.DropTable.TestMapping, schema);

        // Act
        configMapper.Execute(configFile);

        Func<DropTableSystemConfiguration> act = () => configMapper.Execute(configFile);

        // Assert
        act.Should().NotThrow();
    }

    [TestMethod]
    public void CanBuild()
    {
        // Arrange
        DropTableTemplateManager.Templates.Clear();

        var config = new DropTableSystemConfiguration();

        var schema = _mapper.BuildSchema();
        var configMapper = _mapper.CreateMapper(config);

        TomlConfig configFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.DropTable.TestMapping, schema);

        // Act
        configMapper.Execute(configFile);
        config.Build();

        // Assert
        bool isLoaded = DropTableTemplateManager.TryGetTemplate("FirTree_log", out var template);

        isLoaded.Should().BeTrue();
        template.Drops.Should().HaveCountGreaterThan(0);
    }

    [TestMethod]
    public void CanMapDrops()
    {
        // Arrange
        DropTableTemplateManager.Templates.Clear();

        var config = new DropTableSystemConfiguration();

        var schema = _mapper.BuildSchema();
        var configMapper = _mapper.CreateMapper(config);

        TomlConfig configFile = TomlSchemaFileLoader.LoadFile(
            Resources.ResourceManager.DropTable.TestMapping, 
            schema);

        // Act
        configMapper.Execute(configFile);
        config.Build();
        DropTableTemplateManager.TryGetTemplate("FirTree_log", out var template);

        // Assert
        var dropTemplate = template.Drops.First().Value;
        dropTemplate.PrefabName.Should().Be("Guck");
    }
}
