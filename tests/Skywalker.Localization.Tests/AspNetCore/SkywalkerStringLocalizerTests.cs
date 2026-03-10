using System.Globalization;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skywalker.Localization;
using Skywalker.Localization.AspNetCore;
using Xunit;

namespace Skywalker.Localization.Tests.AspNetCore;

public class SkywalkerStringLocalizerTests
{
    private class TestResource { }

    private readonly LocalizationResource _resource;
    private readonly LocalizationOptions _options;
    private readonly IOptions<LocalizationOptions> _optionsAccessor;

    public SkywalkerStringLocalizerTests()
    {
        _resource = new LocalizationResource(typeof(TestResource));
        _options = new LocalizationOptions();
        _optionsAccessor = Options.Create(_options);
    }

    [Fact]
    public void Constructor_ThrowsOnNullResource()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new SkywalkerStringLocalizer(null!, _optionsAccessor));
    }

    [Fact]
    public void Constructor_ThrowsOnNullOptions()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new SkywalkerStringLocalizer(_resource, null!));
    }

    [Fact]
    public void Indexer_WhenNotFound_ReturnsKeyAsValue()
    {
        var localizer = new SkywalkerStringLocalizer(_resource, _optionsAccessor);

        var result = localizer["NotFound"];

        Assert.Equal("NotFound", result.Name);
        Assert.Equal("NotFound", result.Value);
        Assert.True(result.ResourceNotFound);
    }

    [Fact]
    public void Indexer_WhenFound_ReturnsLocalizedValue()
    {
        var contributor = CreateMockContributor("zh-CN", "Hello", "你好");
        _resource.Contributors.Add(contributor);
        var localizer = new SkywalkerStringLocalizer(_resource, _optionsAccessor);

        CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");
        var result = localizer["Hello"];

        Assert.Equal("Hello", result.Name);
        Assert.Equal("你好", result.Value);
        Assert.False(result.ResourceNotFound);
    }

    [Fact]
    public void Indexer_WithArguments_FormatsString()
    {
        var contributor = CreateMockContributor("en", "Welcome", "Welcome, {0}!");
        _resource.Contributors.Add(contributor);
        var localizer = new SkywalkerStringLocalizer(_resource, _optionsAccessor);

        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var result = localizer["Welcome", "User"];

        Assert.Equal("Welcome", result.Name);
        Assert.Equal("Welcome, User!", result.Value);
    }

    [Fact]
    public void GetAllStrings_ReturnsAllStrings()
    {
        var contributor = Substitute.For<ILocalizationResourceContributor>();
        contributor.Fill("en", Arg.Do<Dictionary<string, LocalizedString>>(d =>
        {
            d["Hello"] = new LocalizedString("Hello", "Hello");
            d["Goodbye"] = new LocalizedString("Goodbye", "Goodbye");
        }));
        _resource.Contributors.Add(contributor);
        var localizer = new SkywalkerStringLocalizer(_resource, _optionsAccessor);

        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var result = localizer.GetAllStrings().ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetAllStrings_WithCulture_UsesSpecifiedCulture()
    {
        var contributor = Substitute.For<ILocalizationResourceContributor>();
        contributor.Fill("zh-CN", Arg.Do<Dictionary<string, LocalizedString>>(d =>
        {
            d["Hello"] = new LocalizedString("Hello", "你好");
        }));
        _resource.Contributors.Add(contributor);
        var localizer = new SkywalkerStringLocalizer(_resource, _optionsAccessor);

        var result = localizer.GetAllStrings(new CultureInfo("zh-CN")).ToList();

        Assert.Single(result);
        Assert.Equal("你好", result[0].Value);
    }

    [Fact]
    public void Indexer_FallsBackToParentCulture()
    {
        var contributor = CreateMockContributor("zh", "Hello", "你好");
        _resource.Contributors.Add(contributor);
        var localizer = new SkywalkerStringLocalizer(_resource, _optionsAccessor);

        // zh-CN should fall back to zh
        CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");
        var result = localizer["Hello"];

        Assert.Equal("你好", result.Value);
        Assert.False(result.ResourceNotFound);
    }

    [Fact]
    public void Indexer_UsesGlobalContributors()
    {
        var globalContributor = CreateMockContributor("en", "GlobalKey", "GlobalValue");
        _options.GlobalContributors.Add(globalContributor);
        var localizer = new SkywalkerStringLocalizer(_resource, _optionsAccessor);

        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var result = localizer["GlobalKey"];

        Assert.Equal("GlobalValue", result.Value);
        Assert.False(result.ResourceNotFound);
    }

    private static ILocalizationResourceContributor CreateMockContributor(
        string cultureName, string key, string value)
    {
        var contributor = Substitute.For<ILocalizationResourceContributor>();
        contributor.GetOrNull(cultureName, key).Returns(new LocalizedString(key, value));
        return contributor;
    }
}

