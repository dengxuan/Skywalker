using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skywalker.Template;
using Skywalker.Template.Abstractions;
using Skywalker.Template.Scriban;
using Skywalker.Template.VirtualFiles;
using Xunit;

namespace Skywalker.Template.Tests;

/// <summary>
/// 覆盖 <c>AddTemplate()</c> / <c>AddScribanTemplate()</c> 开箱即用的装配（#310）。
/// 测试中的定义提供者与内容贡献者只登记在 options 中，刻意不注册进 DI。
/// </summary>
public class TemplateServiceCollectionExtensionsTests
{
    private const string TemplateName = "Greeting";

    private sealed class TestTemplateDefinitionProvider : TemplateDefinitionProvider
    {
        public override void Define(ITemplateDefinitionContext context)
        {
            context.Add(new TemplateDefinition(TemplateName));
        }
    }

    private sealed class InMemoryTemplateContentContributor : ITemplateContentContributor
    {
        public Task<string?> GetOrNullAsync(TemplateContentContributorContext context)
        {
            return Task.FromResult<string?>(context.TemplateDefinition.Name == TemplateName ? "Hello, {{ model.name }}!" : null);
        }
    }

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        // Scriban 引擎依赖 Microsoft.Extensions.Localization 的工厂（见 #312）
        services.AddSingleton(Substitute.For<IStringLocalizerFactory>());
        services.AddTemplate(options =>
        {
            options.DefinitionProviders.Add<TestTemplateDefinitionProvider>();
            options.ContentContributors.Add<InMemoryTemplateContentContributor>();
        });
        services.AddScribanTemplate();
        return services.BuildServiceProvider();
    }

    [Fact]
    public void AddTemplate_RegistersVirtualFileContributorBeforeUserContributors()
    {
        using var provider = BuildProvider();

        var options = provider.GetRequiredService<IOptions<SkywalkerTextTemplatingOptions>>().Value;

        Assert.Equal(
            new[] { typeof(VirtualFileTemplateContentContributor), typeof(InMemoryTemplateContentContributor) },
            options.ContentContributors.ToArray());
    }

    [Fact]
    public void AddTemplate_DefaultContributorResolvesFromDi()
    {
        using var provider = BuildProvider();

        Assert.NotNull(provider.GetRequiredService<VirtualFileTemplateContentContributor>());
    }

    [Fact]
    public void AddScribanTemplate_RegistersEngineAsDefault()
    {
        using var provider = BuildProvider();

        var options = provider.GetRequiredService<IOptions<SkywalkerTextTemplatingOptions>>().Value;

        Assert.Equal(ScribanTemplateRenderingEngine.EngineName, options.DefaultRenderingEngine);
        Assert.NotNull(provider.GetRequiredService<ScribanTemplateRenderingEngine>());
    }

    [Fact]
    public async Task RenderAsync_EndToEnd_WithProvidersRegisteredOnlyInOptions()
    {
        using var provider = BuildProvider();

        var renderer = provider.GetRequiredService<ITemplateRenderer>();

        var result = await renderer.RenderAsync(TemplateName, new { Name = "World" });

        Assert.Equal("Hello, World!", result);
    }
}
