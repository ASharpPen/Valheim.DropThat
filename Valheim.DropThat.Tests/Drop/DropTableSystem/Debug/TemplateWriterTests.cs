using DropThat.Drop.DropTableSystem.Managers;
using DropThat.Drop.DropTableSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThatCore.Config.Toml.Schema;
using ThatCore.Config.Toml;
using DropThat.Drop.DropTableSystem.Configuration.Toml;
using System.Linq;
using DropThat.Drop.DropTableSystem.Debug;
using FluentAssertions;
using ThatCore.Logging;

namespace Valheim.DropThat.Tests.Drop.DropTableSystem.Debug;

[TestClass]
public class TemplateWriterTests
{
    private DropTableConfigMapper _mapper;

    [TestInitialize()]
    public void Initialize()
    {
        _mapper = ConfigurationFileManager.RegisterMainMappings();
    }

    [TestMethod]
    public void CanWriteDropTemplatesToConfig()
    {
        // Arrange
        DropTableTemplateManager.Templates.Clear();

        var config = new DropTableSystemConfiguration();

        var schema = _mapper.BuildSchema();
        var configMapper = _mapper.CreateMapper(config);

        TomlConfig configFile = TomlSchemaFileLoader.LoadFile(Resources.ResourceManager.DropTable.TestMapping2, schema);

        configMapper.Execute(configFile);
        config.Build();

        var templates = DropTableTemplateManager.Templates.Values.ToList();

        // Act
        var configResult = _mapper.MapToConfigFromTemplates(templates);
        var content = TomlConfigWriter.WriteToString(configResult, new());

        // Assert
        content.Should().NotBeNullOrWhiteSpace();
    }
}
