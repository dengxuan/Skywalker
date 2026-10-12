using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skywalker.Template;
using Skywalker.Template.Abstractions;
using Xunit;

namespace Skywalker.Template.Tests;

public class TemplateContentProviderTests
{
    /// <summary>模拟单文件模板读取器：只响应 culture 为 null 的查询。</summary>
    private sealed class CultureIndependentContributor : ITemplateContentContributor
    {
        public Task<string?> GetOrNullAsync(TemplateContentContributorContext context)
            => Task.FromResult(context.Culture == null ? "content" : null);
    }

    private static TemplateContentProvider CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton<CultureIndependentContributor>();
        var serviceProvider = services.BuildServiceProvider();

        var options = new SkywalkerTextTemplatingOptions();
        options.ContentContributors.Add<CultureIndependentContributor>();

        return new TemplateContentProvider(
            Substitute.For<ITemplateDefinitionManager>(),
            serviceProvider.GetRequiredService<IServiceScopeFactory>(),
            Options.Create(options));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetContentOrNullAsync_SingleFileTemplate_FoundRegardlessOfInlineLocalized(bool isInlineLocalized)
    {
        var definition = new TemplateDefinition("Single") { IsInlineLocalized = isInlineLocalized };

        var content = await CreateProvider().GetContentOrNullAsync(definition, "zh-CN");

        Assert.Equal("content", content);
    }

    [Fact]
    public async Task GetContentOrNullAsync_TryDefaultsFalse_DoesNotFallBack()
    {
        var definition = new TemplateDefinition("Single");

        var content = await CreateProvider().GetContentOrNullAsync(definition, "zh-CN", tryDefaults: false);

        Assert.Null(content);
    }
}
