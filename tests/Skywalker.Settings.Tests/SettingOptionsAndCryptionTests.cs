using System.Text;
using Skywalker.Settings;
using Xunit;

namespace Skywalker.Settings.Tests;

public class SettingOptionsAndCryptionTests
{
    // SkywalkerSettingOptions
    [Fact]
    public void SkywalkerSettingOptions_HasValueProvidersList()
    {
        var options = new SkywalkerSettingOptions();
        Assert.NotNull(options.ValueProviders);
        Assert.Empty(options.ValueProviders);
    }

    // SettingCryptionOptions
    [Fact]
    public void SettingCryptionOptions_Defaults()
    {
        var options = new SettingCryptionOptions();
        Assert.Equal(256, options.Keysize);
        Assert.Equal("gsKnGZ041HLL4IM8", options.DefaultPassPhrase);
        Assert.Equal(Encoding.ASCII.GetBytes("jkE49230Tf093b42"), options.InitVectorBytes);
        Assert.Equal(Encoding.ASCII.GetBytes("hgt!16kl"), options.DefaultSalt);
    }

    [Fact]
    public void SettingCryptionOptions_CanBeModified()
    {
        var options = new SettingCryptionOptions();
        options.Keysize = 128;
        options.DefaultPassPhrase = "custom";
        Assert.Equal(128, options.Keysize);
        Assert.Equal("custom", options.DefaultPassPhrase);
    }
}
