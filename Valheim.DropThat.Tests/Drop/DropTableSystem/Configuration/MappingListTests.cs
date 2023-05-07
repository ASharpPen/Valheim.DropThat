using System.Linq;
using DropThat.Drop.DropTableSystem;
using DropThat.Drop.DropTableSystem.Configuration.Toml;
using DropThat.Drop.DropTableSystem.Managers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThatCore.Config.Toml.Schema;

namespace Valheim.DropThat.Tests.Drop.DropTableSystem.Configuration;

[TestClass]
public class MappingListTests
{
    private DropTableConfigMapper _mapper;
    private DropTableListConfigMapper _listMapper;

    [TestInitialize()]
    public void Initialize()
    {
        if (_mapper is null)
        {
            _mapper = ConfigurationFileManager.RegisterMainMappings();
        }

        if (_listMapper is null)
        {
            _listMapper = ConfigurationFileManager.RegisterListMappings();
        }
    }

    [TestMethod]
    public void CanLoadFile()
    {
        // Arrange
        var schema = _listMapper.BuildSchema();

        // Act
        var configFile = TomlSchemaFileLoader.LoadFile(
            Resources.ResourceManager.DropTable.TestListMapping, 
            schema);

        // Assert
        configFile.Should().NotBeNull();
    }

    [TestMethod]
    public void CanMergeWithMain()
    {
        // Arrange
        DropTableTemplateManager.Templates.Clear();

        var listSchema = _listMapper.BuildSchema();
        var mainSchema = _mapper.BuildSchema();

        var listToml = TomlSchemaFileLoader.LoadFile(
            Resources.ResourceManager.DropTable.TestListMapping,
            listSchema);
        var mainToml = TomlSchemaFileLoader.LoadFile(
            Resources.ResourceManager.DropTable.TestMapping,
            mainSchema);

        // Act
        var mergedConfig = ConfigurationFileManager.MergeListAndMain(listToml, mainToml);

        // Assert
        mergedConfig.Sections.Should().HaveCount(1); // One drop table entry

        var dropTableSection = mergedConfig.Sections.Values.First();
        dropTableSection.Sections.Should().HaveCount(
            3, 
            "two drops were merged from list, two from main, with one overlapping");
    }

    [TestMethod]
    public void CanMapTableSettings()
    {
        // Arrange
        DropTableTemplateManager.Templates.Clear();

        var configSystem = new DropTableSystemConfiguration();

        var listSchema = _listMapper.BuildSchema();
        var mainSchema = _mapper.BuildSchema();

        var templateMapper = _mapper.CreateMapper(configSystem);

        var listToml = TomlSchemaFileLoader.LoadFile(
            Resources.ResourceManager.DropTable.TestListMapping,
            listSchema);
        var mainToml = TomlSchemaFileLoader.LoadFile(
            Resources.ResourceManager.DropTable.TestMapping,
            mainSchema);

        // Act
        var mergedConfig = ConfigurationFileManager.MergeListAndMain(listToml, mainToml);

        // Assert
        mergedConfig.Sections.Should().HaveCount(1); // One drop table entry

        var dropTableSection = mergedConfig.Sections.Values.First();

        dropTableSection.Settings.Should().HaveCount(5, "5 settings are defined in test_Mapping file");
        dropTableSection.Settings["SetDropChance"].IsSet.Should().BeTrue();
        dropTableSection.Settings["SetDropChance"].GetValue().Should().Be(70);
    }

    [TestMethod]
    public void CanMapTableDrops()
    {
        // Arrange
        DropTableTemplateManager.Templates.Clear();

        var configSystem = new DropTableSystemConfiguration();

        var listSchema = _listMapper.BuildSchema();
        var mainSchema = _mapper.BuildSchema();

        var templateMapper = _mapper.CreateMapper(configSystem);

        var listToml = TomlSchemaFileLoader.LoadFile(
            Resources.ResourceManager.DropTable.TestListMapping,
            listSchema);
        var mainToml = TomlSchemaFileLoader.LoadFile(
            Resources.ResourceManager.DropTable.TestMapping,
            mainSchema);

        // Act
        var mergedConfig = ConfigurationFileManager.MergeListAndMain(listToml, mainToml);
        templateMapper.Execute(mergedConfig);
        configSystem.Build();

        // Assert
        var template = DropTableTemplateManager.GetTemplate("FirTree_log");
        template.Should().NotBeNull();

        template.Drops.Should().HaveCount(
            3,
            "two drops were merged from list, two from main, with one overlapping");

        template.Drops[0].PrefabName.Value.Should().Be("RawMeat", "added by list");
        template.Drops[3].PrefabName.Value.Should().Be("Guck", "added by list, overridden by main");
        template.Drops[4].PrefabName.Value.Should().Be("Guck", "added by main");
    }

    [TestMethod]
    public void CanMergeAndMapMultipleLists()
    {
        // Arrange
        DropTableTemplateManager.Templates.Clear();

        var configSystem = new DropTableSystemConfiguration();

        var listSchema = _listMapper.BuildSchema();
        var mainSchema = _mapper.BuildSchema();

        var templateMapper = _mapper.CreateMapper(configSystem);

        var listToml = TomlSchemaFileLoader.LoadFile(
            Resources.ResourceManager.DropTable.TestListMapping,
            listSchema);
        var listToml2 = TomlSchemaFileLoader.LoadFile(
            Resources.ResourceManager.DropTable.TestListMapping2,
            listSchema);
        var mainToml = TomlSchemaFileLoader.LoadFile(
            Resources.ResourceManager.DropTable.TestMapping,
            mainSchema);

        // Act
        var mergedList = ConfigurationFileManager.MergeConfigs(new[] { listToml, listToml2 });

        var mergedConfig = ConfigurationFileManager.MergeListAndMain(mergedList, mainToml);
        templateMapper.Execute(mergedConfig);
        configSystem.Build();

        // Assert
        var template = DropTableTemplateManager.GetTemplate("FirTree_log");
        template.Should().NotBeNull();

        template.Drops.Should().HaveCount(
            4,
            "three drops were merged from list, two from main, with one overlapping");

        template.Drops[0].PrefabName.Value.Should().Be("IronScrap", "added by list, overridden by list 2");
        template.Drops[1].PrefabName.Value.Should().Be("IronScrap", "added by list 2");
        template.Drops[3].PrefabName.Value.Should().Be("Guck", "added by list, overridden by main");
        template.Drops[4].PrefabName.Value.Should().Be("Guck", "added by main");
    }
}
