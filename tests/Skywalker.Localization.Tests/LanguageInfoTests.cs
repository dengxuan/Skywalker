using Skywalker.Localization;
using Xunit;

namespace Skywalker.Localization.Tests;

public class LanguageInfoTests
{
    [Fact]
    public void Constructor_SetsCultureName()
    {
        var language = new LanguageInfo("en-US", "English");

        Assert.Equal("en-US", language.CultureName);
    }

    [Fact]
    public void Constructor_SetsDisplayName()
    {
        var language = new LanguageInfo("en-US", "English");

        Assert.Equal("English", language.DisplayName);
    }

    [Fact]
    public void Constructor_UiCultureName_DefaultsToCultureName()
    {
        var language = new LanguageInfo("en-US", "English");

        Assert.Equal("en-US", language.UiCultureName);
    }

    [Fact]
    public void Constructor_UiCultureName_CanBeOverridden()
    {
        var language = new LanguageInfo("en-US", "English", uiCultureName: "en");

        Assert.Equal("en", language.UiCultureName);
    }

    [Fact]
    public void FlagIcon_CanBeSet()
    {
        var language = new LanguageInfo("en-US", "English", flagIcon: "us");

        Assert.Equal("us", language.FlagIcon);
    }

    [Fact]
    public void IsDefault_DefaultsToFalse()
    {
        var language = new LanguageInfo("en-US", "English");

        Assert.False(language.IsDefault);
    }

    [Fact]
    public void IsDefault_CanBeSetInConstructor()
    {
        var language = new LanguageInfo("en-US", "English", isDefault: true);

        Assert.True(language.IsDefault);
    }

    [Fact]
    public void IsEnabled_DefaultsToTrue()
    {
        var language = new LanguageInfo("en-US", "English");

        Assert.True(language.IsEnabled);
    }

    [Fact]
    public void Constructor_ThrowsOnNullCultureName()
    {
        Assert.Throws<ArgumentNullException>(() => new LanguageInfo(null!, "English"));
    }

    [Fact]
    public void Constructor_ThrowsOnNullDisplayName()
    {
        Assert.Throws<ArgumentNullException>(() => new LanguageInfo("en-US", null!));
    }
}

