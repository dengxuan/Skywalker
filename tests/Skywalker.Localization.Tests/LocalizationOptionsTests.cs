using NSubstitute;
using Skywalker.Localization;
using Xunit;

namespace Skywalker.Localization.Tests;

public class LocalizationOptionsTests
{
    private class TestResource { }
    private class DefaultResource { }

    [Fact]
    public void Constructor_InitializesResources()
    {
        var options = new LocalizationOptions();

        Assert.NotNull(options.Resources);
        Assert.Empty(options.Resources);
    }

    [Fact]
    public void Constructor_InitializesGlobalContributors()
    {
        var options = new LocalizationOptions();

        Assert.NotNull(options.GlobalContributors);
        Assert.Empty(options.GlobalContributors);
    }

    [Fact]
    public void Constructor_InitializesLanguages()
    {
        var options = new LocalizationOptions();

        Assert.NotNull(options.Languages);
        Assert.Empty(options.Languages);
    }

    [Fact]
    public void DefaultResourceType_DefaultsToNull()
    {
        var options = new LocalizationOptions();

        Assert.Null(options.DefaultResourceType);
    }

    [Fact]
    public void DefaultResourceType_CanBeSet()
    {
        var options = new LocalizationOptions();

        options.DefaultResourceType = typeof(DefaultResource);

        Assert.Equal(typeof(DefaultResource), options.DefaultResourceType);
    }

    [Fact]
    public void Resources_CanAddResource()
    {
        var options = new LocalizationOptions();

        var resource = options.Resources.Add<TestResource>();

        Assert.NotNull(resource);
        Assert.Single(options.Resources);
    }

    [Fact]
    public void GlobalContributors_CanAddContributor()
    {
        var options = new LocalizationOptions();
        var contributor = Substitute.For<ILocalizationResourceContributor>();

        options.GlobalContributors.Add(contributor);

        Assert.Single(options.GlobalContributors);
        Assert.Same(contributor, options.GlobalContributors[0]);
    }

    [Fact]
    public void Languages_CanAddLanguage()
    {
        var options = new LocalizationOptions();

        options.Languages.Add(new LanguageInfo("en-US", "English"));
        options.Languages.Add(new LanguageInfo("zh-CN", "简体中文"));

        Assert.Equal(2, options.Languages.Count);
        Assert.Equal("en-US", options.Languages[0].CultureName);
        Assert.Equal("zh-CN", options.Languages[1].CultureName);
    }

    [Fact]
    public void Languages_CanSetDefaultLanguage()
    {
        var options = new LocalizationOptions();

        options.Languages.Add(new LanguageInfo("en-US", "English", isDefault: true));
        options.Languages.Add(new LanguageInfo("zh-CN", "简体中文"));

        var defaultLanguage = options.Languages.FirstOrDefault(l => l.IsDefault);
        Assert.NotNull(defaultLanguage);
        Assert.Equal("en-US", defaultLanguage.CultureName);
    }

    [Fact]
    public void Languages_CanDisableLanguage()
    {
        var options = new LocalizationOptions();
        var language = new LanguageInfo("en-US", "English");

        options.Languages.Add(language);
        language.IsEnabled = false;

        Assert.False(options.Languages[0].IsEnabled);
    }

    [Fact]
    public void Resources_WithDefaultCulture_SetsDefaultCultureName()
    {
        var options = new LocalizationOptions();

        var resource = options.Resources.Add<TestResource>("en");

        Assert.Equal("en", resource.DefaultCultureName);
    }

    [Fact]
    public void GlobalContributors_ApplyToAllResources()
    {
        var options = new LocalizationOptions();
        var contributor1 = Substitute.For<ILocalizationResourceContributor>();
        var contributor2 = Substitute.For<ILocalizationResourceContributor>();

        options.GlobalContributors.Add(contributor1);
        options.GlobalContributors.Add(contributor2);

        Assert.Equal(2, options.GlobalContributors.Count);
    }
}

