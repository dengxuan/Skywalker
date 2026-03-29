using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Skywalker.Settings;
using Skywalker.Settings.Abstractions;

namespace Skywalker.Settings.Tests;

public class SettingProviderTests
{
    [Fact]
    public async Task GetOrNullAsync_ReturnsNull_WhenSettingNotDefined()
    {
        var defMgr = Substitute.For<ISettingDefinitionManager>();
        defMgr.GetOrNull("undefined").Returns((SettingDefinition?)null);

        var encSvc = Substitute.For<ISettingEncryptionService>();
        var pvdMgr = Substitute.For<ISettingValueProviderManager>();

        var provider = new SettingProvider(defMgr, encSvc, pvdMgr);

        var result = await provider.GetOrNullAsync("undefined");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetOrNullAsync_ReturnsValue_FromFirstProviderWithValue()
    {
        var setting = new SettingDefinition("MySetting");
        var defMgr = Substitute.For<ISettingDefinitionManager>();
        defMgr.GetOrNull("MySetting").Returns(setting);

        var pvd1 = Substitute.For<ISettingValueProvider>();
        pvd1.Name.Returns("P1");
        pvd1.GetOrNullAsync(setting).Returns((string?)null);

        var pvd2 = Substitute.For<ISettingValueProvider>();
        pvd2.Name.Returns("P2");
        pvd2.GetOrNullAsync(setting).Returns("found-value");

        var pvdMgr = Substitute.For<ISettingValueProviderManager>();
        pvdMgr.Providers.Returns(new List<ISettingValueProvider> { pvd1, pvd2 });

        var encSvc = Substitute.For<ISettingEncryptionService>();
        var provider = new SettingProvider(defMgr, encSvc, pvdMgr);

        var result = await provider.GetOrNullAsync("MySetting");

        Assert.Equal("found-value", result);
    }

    [Fact]
    public async Task GetOrNullAsync_DecryptsValue_WhenSettingIsEncrypted()
    {
        var setting = new SettingDefinition("Secret", isEncrypted: true);
        var defMgr = Substitute.For<ISettingDefinitionManager>();
        defMgr.GetOrNull("Secret").Returns(setting);

        var pvd = Substitute.For<ISettingValueProvider>();
        pvd.Name.Returns("P1");
        pvd.GetOrNullAsync(setting).Returns("encrypted-data");

        var pvdMgr = Substitute.For<ISettingValueProviderManager>();
        pvdMgr.Providers.Returns(new List<ISettingValueProvider> { pvd });

        var encSvc = Substitute.For<ISettingEncryptionService>();
        encSvc.Decrypt("encrypted-data").Returns("decrypted-data");

        var provider = new SettingProvider(defMgr, encSvc, pvdMgr);

        var result = await provider.GetOrNullAsync("Secret");

        Assert.Equal("decrypted-data", result);
    }

    [Fact]
    public async Task GetAsync_ThrowsKeyNotFoundException_WhenValueIsNull()
    {
        var defMgr = Substitute.For<ISettingDefinitionManager>();
        defMgr.GetOrNull("missing").Returns((SettingDefinition?)null);

        var encSvc = Substitute.For<ISettingEncryptionService>();
        var pvdMgr = Substitute.For<ISettingValueProviderManager>();

        var provider = new SettingProvider(defMgr, encSvc, pvdMgr);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => provider.GetAsync("missing"));
    }

    [Fact]
    public async Task GetOrNullAsync_FiltersProviders_BySettingAllowedProviders()
    {
        var setting = new SettingDefinition("MySetting").WithProviders("P2");
        var defMgr = Substitute.For<ISettingDefinitionManager>();
        defMgr.GetOrNull("MySetting").Returns(setting);

        var pvd1 = Substitute.For<ISettingValueProvider>();
        pvd1.Name.Returns("P1");
        pvd1.GetOrNullAsync(setting).Returns("from-p1");

        var pvd2 = Substitute.For<ISettingValueProvider>();
        pvd2.Name.Returns("P2");
        pvd2.GetOrNullAsync(setting).Returns("from-p2");

        var pvdMgr = Substitute.For<ISettingValueProviderManager>();
        pvdMgr.Providers.Returns(new List<ISettingValueProvider> { pvd1, pvd2 });

        var encSvc = Substitute.For<ISettingEncryptionService>();
        var provider = new SettingProvider(defMgr, encSvc, pvdMgr);

        var result = await provider.GetOrNullAsync("MySetting");

        Assert.Equal("from-p2", result);
        await pvd1.DidNotReceive().GetOrNullAsync(setting);
    }
}

public class DefaultValueSettingValueProviderTests
{
    [Fact]
    public async Task GetOrNullAsync_ReturnsDefaultValue()
    {
        var provider = new DefaultValueSettingValueProvider();
        var setting = new SettingDefinition("Test", defaultValue: "default-val");

        var result = await provider.GetOrNullAsync(setting);

        Assert.Equal("default-val", result);
    }

    [Fact]
    public async Task GetOrNullAsync_ReturnsNull_WhenNoDefault()
    {
        var provider = new DefaultValueSettingValueProvider();
        var setting = new SettingDefinition("Test");

        var result = await provider.GetOrNullAsync(setting);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsDefaultValues()
    {
        var provider = new DefaultValueSettingValueProvider();
        var settings = new[]
        {
            new SettingDefinition("A", defaultValue: "va"),
            new SettingDefinition("B", defaultValue: "vb"),
        };

        var result = await provider.GetAllAsync(settings);

        Assert.Equal(2, result.Count);
        Assert.Equal("va", result[0].Value);
        Assert.Equal("vb", result[1].Value);
    }

    [Fact]
    public void Name_IsD()
    {
        var provider = new DefaultValueSettingValueProvider();
        Assert.Equal("D", provider.Name);
    }
}

public class ConfigurationSettingValueProviderTests
{
    [Fact]
    public async Task GetOrNullAsync_ReadsFromConfiguration()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Settings:MySetting", "config-value" }
            })
            .Build();

        var provider = new ConfigurationSettingValueProvider(config);
        var setting = new SettingDefinition("MySetting");

        var result = await provider.GetOrNullAsync(setting);

        Assert.Equal("config-value", result);
    }

    [Fact]
    public async Task GetOrNullAsync_ReturnsNull_WhenNotInConfiguration()
    {
        var config = new ConfigurationBuilder().Build();
        var provider = new ConfigurationSettingValueProvider(config);
        var setting = new SettingDefinition("Missing");

        var result = await provider.GetOrNullAsync(setting);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReadsAllFromConfiguration()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Settings:A", "va" },
                { "Settings:B", "vb" },
            })
            .Build();

        var provider = new ConfigurationSettingValueProvider(config);
        var settings = new[]
        {
            new SettingDefinition("A"),
            new SettingDefinition("B"),
        };

        var result = await provider.GetAllAsync(settings);

        Assert.Equal("va", result[0].Value);
        Assert.Equal("vb", result[1].Value);
    }

    [Fact]
    public void Name_IsC()
    {
        var config = new ConfigurationBuilder().Build();
        var provider = new ConfigurationSettingValueProvider(config);
        Assert.Equal("C", provider.Name);
    }
}

public class SettingDefinitionContextTests
{
    [Fact]
    public void Add_AddsSettingDefinitions()
    {
        var dict = new Dictionary<string, SettingDefinition>();
        var context = new SettingDefinitionContext(dict);

        context.Add(new SettingDefinition("A"), new SettingDefinition("B"));

        Assert.Equal(2, dict.Count);
    }

    [Fact]
    public void Add_OverwritesDuplicateName()
    {
        var dict = new Dictionary<string, SettingDefinition>();
        var context = new SettingDefinitionContext(dict);

        context.Add(new SettingDefinition("A", defaultValue: "v1"));
        context.Add(new SettingDefinition("A", defaultValue: "v2"));

        Assert.Single(dict);
        Assert.Equal("v2", dict["A"].DefaultValue);
    }

    [Fact]
    public void GetOrNull_ReturnsDefinition()
    {
        var dict = new Dictionary<string, SettingDefinition>
        {
            { "A", new SettingDefinition("A") }
        };
        var context = new SettingDefinitionContext(dict);

        Assert.NotNull(context.GetOrNull("A"));
        Assert.Null(context.GetOrNull("B"));
    }

    [Fact]
    public void GetAll_ReturnsAllDefinitions()
    {
        var dict = new Dictionary<string, SettingDefinition>
        {
            { "A", new SettingDefinition("A") },
            { "B", new SettingDefinition("B") },
        };
        var context = new SettingDefinitionContext(dict);

        Assert.Equal(2, context.GetAll().Count);
    }
}

public class SettingEncryptionServiceTests
{
    [Fact]
    public void Encrypt_Decrypt_Roundtrip()
    {
        var options = Options.Create(new SettingCryptionOptions());
        var service = new SettingEncryptionService(options);

        var encrypted = service.Encrypt("hello world");
        Assert.NotNull(encrypted);
        Assert.NotEqual("hello world", encrypted);

        var decrypted = service.Decrypt(encrypted);
        Assert.Equal("hello world", decrypted);
    }

    [Fact]
    public void Encrypt_NullOrEmpty_ReturnsAsIs()
    {
        var options = Options.Create(new SettingCryptionOptions());
        var service = new SettingEncryptionService(options);

        Assert.Null(service.Encrypt(null));
        Assert.Equal(string.Empty, service.Encrypt(string.Empty));
    }

    [Fact]
    public void Decrypt_NullOrEmpty_ReturnsAsIs()
    {
        var options = Options.Create(new SettingCryptionOptions());
        var service = new SettingEncryptionService(options);

        Assert.Null(service.Decrypt(null));
        Assert.Equal(string.Empty, service.Decrypt(string.Empty));
    }

    [Fact]
    public void Decrypt_InvalidCipherText_ReturnsNull()
    {
        var options = Options.Create(new SettingCryptionOptions());
        var service = new SettingEncryptionService(options);

        var result = service.Decrypt("not-a-valid-base64-cipher");
        Assert.Null(result);
    }
}
