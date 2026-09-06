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
    public void Indexer_MissingInCurrentChain_FallsBackToResourceDefaultCulture()
    {
        // en-US 缺 key、zh-CN 有：拿中文而不是把 key 露到界面上；ResourceNotFound 仍为 true 让调用方知道这是回落值
        _resource.DefaultCultureName = "zh-CN";
        _resource.Contributors.Add(CreateMockContributor("zh-CN", "Hello", "你好"));
        var localizer = new SkywalkerStringLocalizer(_resource, _optionsAccessor);

        CultureInfo.CurrentUICulture = new CultureInfo("en-US");
        var result = localizer["Hello"];

        Assert.Equal("你好", result.Value);
        Assert.True(result.ResourceNotFound);
    }

    [Fact]
    public void Indexer_MissingInCurrentChain_FallsBackToGlobalDefaultCulture()
    {
        _options.DefaultCultureName = "zh-CN";
        _resource.Contributors.Add(CreateMockContributor("zh-CN", "Hello", "你好"));
        var localizer = new SkywalkerStringLocalizer(_resource, _optionsAccessor);

        CultureInfo.CurrentUICulture = new CultureInfo("en-US");

        Assert.Equal("你好", localizer["Hello"].Value);
    }

    [Fact]
    public void Indexer_MissingEverywhere_ReturnsKey_EvenWithDefaultCulture()
    {
        _options.DefaultCultureName = "zh-CN";
        var localizer = new SkywalkerStringLocalizer(_resource, _optionsAccessor);

        CultureInfo.CurrentUICulture = new CultureInfo("en-US");
        var result = localizer["Nope"];

        Assert.Equal("Nope", result.Value);
        Assert.True(result.ResourceNotFound);
    }

    [Fact]
    public void Indexer_CurrentCultureHit_WinsOverDefaultCulture()
    {
        _options.DefaultCultureName = "zh-CN";
        _resource.Contributors.Add(CreateMockContributor("zh-CN", "Hello", "你好"));
        _resource.Contributors.Add(CreateMockContributor("en-US", "Hello", "Hello"));
        var localizer = new SkywalkerStringLocalizer(_resource, _optionsAccessor);

        CultureInfo.CurrentUICulture = new CultureInfo("en-US");
        var result = localizer["Hello"];

        Assert.Equal("Hello", result.Value);
        Assert.False(result.ResourceNotFound);
    }

    [Fact]
    public void Indexer_NamedPlaceholders_FromAnonymousObject()
    {
        _resource.Contributors.Add(CreateMockContributor("zh-CN", "DaysLeft", "剩余 {n} 天，共 {total} 天"));
        var localizer = new SkywalkerStringLocalizer(_resource, _optionsAccessor);
        CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");

        Assert.Equal("剩余 3 天，共 30 天", localizer["DaysLeft", new { n = 3, total = 30 }].Value);
    }

    [Fact]
    public void Indexer_NamedPlaceholders_FromDictionary_MissingArgumentKept()
    {
        // 漏传的参数原样保留：界面露出 "{total}" 一眼可见，空串是静默失败
        _resource.Contributors.Add(CreateMockContributor("zh-CN", "DaysLeft", "剩余 {n} 天，共 {total} 天"));
        var localizer = new SkywalkerStringLocalizer(_resource, _optionsAccessor);
        CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");

        var args = new Dictionary<string, object?> { ["n"] = 3 };
        Assert.Equal("剩余 3 天，共 {total} 天", localizer["DaysLeft", args].Value);
    }

    [Fact]
    public void Indexer_PositionalPlaceholders_StillWork()
    {
        _resource.Contributors.Add(CreateMockContributor("zh-CN", "Hi", "你好 {0}，今天 {1}"));
        var localizer = new SkywalkerStringLocalizer(_resource, _optionsAccessor);
        CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");

        Assert.Equal("你好 张三，今天 周一", localizer["Hi", "张三", "周一"].Value);
        Assert.Equal("你好 3，今天 {1}", localizer["Hi", 3, "{1}"].Value);   // 单个数字参数走位置占位，不走命名
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

