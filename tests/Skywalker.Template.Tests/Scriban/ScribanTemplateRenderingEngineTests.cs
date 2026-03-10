using Microsoft.Extensions.Localization;
using NSubstitute;
using Skywalker.Template;
using Skywalker.Template.Abstractions;
using Skywalker.Template.Scriban;
using Xunit;

namespace Skywalker.Template.Tests.Scriban;

public class ScribanTemplateRenderingEngineTests
{
    private readonly ITemplateDefinitionManager _templateDefinitionManager;
    private readonly ITemplateContentProvider _templateContentProvider;
    private readonly IStringLocalizerFactory _stringLocalizerFactory;
    private readonly IStringLocalizer _stringLocalizer;
    private readonly ScribanTemplateRenderingEngine _engine;

    public ScribanTemplateRenderingEngineTests()
    {
        _templateDefinitionManager = Substitute.For<ITemplateDefinitionManager>();
        _templateContentProvider = Substitute.For<ITemplateContentProvider>();
        _stringLocalizerFactory = Substitute.For<IStringLocalizerFactory>();
        _stringLocalizer = Substitute.For<IStringLocalizer>();

        _stringLocalizerFactory.Create(Arg.Any<Type>()).Returns(_stringLocalizer);
        _stringLocalizerFactory.Create(Arg.Any<string>(), Arg.Any<string>()).Returns(_stringLocalizer);

        _engine = new ScribanTemplateRenderingEngine(
            _templateDefinitionManager,
            _templateContentProvider,
            _stringLocalizerFactory);
    }

    [Fact]
    public void Name_ShouldBeScriban()
    {
        Assert.Equal("Scriban", _engine.Name);
    }

    [Fact]
    public async Task RenderAsync_SimpleTemplate_ReturnsRenderedContent()
    {
        // Arrange
        var templateName = "TestTemplate";
        var templateContent = "Hello, {{ model.name }}!";
        var model = new { Name = "World" };

        var templateDefinition = new TemplateDefinition(templateName);
        _templateDefinitionManager.Get(templateName).Returns(templateDefinition);
        _templateContentProvider.GetContentOrNullAsync(templateDefinition, Arg.Any<string?>(), Arg.Any<bool>(), Arg.Any<bool>())
            .Returns(Task.FromResult<string?>(templateContent));

        // Act
        var result = await _engine.RenderAsync(templateName, model);

        // Assert
        Assert.Equal("Hello, World!", result);
    }

    [Fact]
    public async Task RenderAsync_WithGlobalContext_IncludesContextVariables()
    {
        // Arrange
        var templateName = "TestTemplate";
        var templateContent = "Hello, {{ name }}!";

        var templateDefinition = new TemplateDefinition(templateName);
        _templateDefinitionManager.Get(templateName).Returns(templateDefinition);
        _templateContentProvider.GetContentOrNullAsync(templateDefinition, Arg.Any<string?>(), Arg.Any<bool>(), Arg.Any<bool>())
            .Returns(Task.FromResult<string?>(templateContent));

        var globalContext = new Dictionary<string, object> { { "name", "Test" } };

        // Act
        var result = await _engine.RenderAsync(templateName, null, null, globalContext);

        // Assert
        Assert.Equal("Hello, Test!", result);
    }

    [Fact]
    public async Task RenderAsync_WithLocalizer_CallsLocalizerFunction()
    {
        // Arrange
        var templateName = "TestTemplate";
        var templateContent = "{{ L 'Greeting' }}";

        var templateDefinition = new TemplateDefinition(templateName);
        _templateDefinitionManager.Get(templateName).Returns(templateDefinition);
        _templateContentProvider.GetContentOrNullAsync(templateDefinition, Arg.Any<string?>(), Arg.Any<bool>(), Arg.Any<bool>())
            .Returns(Task.FromResult<string?>(templateContent));

        _stringLocalizer["Greeting"].Returns(new LocalizedString("Greeting", "Hello!"));

        // Act
        var result = await _engine.RenderAsync(templateName);

        // Assert
        Assert.Equal("Hello!", result);
    }

    [Fact]
    public async Task RenderAsync_TemplateNotFound_ThrowsException()
    {
        // Arrange
        var templateName = "TestTemplate";

        var templateDefinition = new TemplateDefinition(templateName);
        _templateDefinitionManager.Get(templateName).Returns(templateDefinition);
        _templateContentProvider.GetContentOrNullAsync(templateDefinition, Arg.Any<string?>(), Arg.Any<bool>(), Arg.Any<bool>())
            .Returns(Task.FromResult<string?>(null));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _engine.RenderAsync(templateName));
    }
}

