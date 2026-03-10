using Microsoft.Extensions.Localization;
using NSubstitute;
using Skywalker.Template;
using Skywalker.Template.Abstractions;
using Skywalker.Template.Razor;
using Xunit;

namespace Skywalker.Template.Tests.Razor;

public class RazorTemplateRenderingEngineTests
{
    private readonly ITemplateDefinitionManager _templateDefinitionManager;
    private readonly ITemplateContentProvider _templateContentProvider;
    private readonly IStringLocalizerFactory _stringLocalizerFactory;
    private readonly IStringLocalizer _stringLocalizer;
    private readonly RazorTemplateRenderingEngine _engine;

    public RazorTemplateRenderingEngineTests()
    {
        _templateDefinitionManager = Substitute.For<ITemplateDefinitionManager>();
        _templateContentProvider = Substitute.For<ITemplateContentProvider>();
        _stringLocalizerFactory = Substitute.For<IStringLocalizerFactory>();
        _stringLocalizer = Substitute.For<IStringLocalizer>();

        _stringLocalizerFactory.Create(Arg.Any<Type>()).Returns(_stringLocalizer);
        _stringLocalizerFactory.Create(Arg.Any<string>(), Arg.Any<string>()).Returns(_stringLocalizer);

        _engine = new RazorTemplateRenderingEngine(
            _templateDefinitionManager,
            _templateContentProvider,
            _stringLocalizerFactory);
    }

    [Fact]
    public void Name_ShouldBeRazor()
    {
        Assert.Equal("Razor", _engine.Name);
    }

    [Fact(Skip = "RazorLight requires PreserveCompilationContext which is not available in test environment")]
    public async Task RenderAsync_SimpleTemplate_ReturnsRenderedContent()
    {
        // Arrange
        var templateName = "TestTemplate";
        var templateContent = "Hello, @Model.Name!";
        var model = new TestModel { Name = "World" };

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

    [Fact(Skip = "RazorLight requires PreserveCompilationContext which is not available in test environment")]
    public async Task RenderAsync_WithEmptyModel_Succeeds()
    {
        // Arrange
        var templateName = "TestTemplate";
        var templateContent = "Hello, World!";

        var templateDefinition = new TemplateDefinition(templateName);
        _templateDefinitionManager.Get(templateName).Returns(templateDefinition);
        _templateContentProvider.GetContentOrNullAsync(templateDefinition, Arg.Any<string?>(), Arg.Any<bool>(), Arg.Any<bool>())
            .Returns(Task.FromResult<string?>(templateContent));

        // Act
        var result = await _engine.RenderAsync(templateName);

        // Assert
        Assert.Equal("Hello, World!", result);
    }

    public class TestModel
    {
        public string Name { get; set; } = string.Empty;
    }
}

