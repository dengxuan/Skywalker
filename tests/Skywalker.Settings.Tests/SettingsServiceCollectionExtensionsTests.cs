using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.Settings;
using Skywalker.Settings.Abstractions;

namespace Skywalker.Settings.Tests;

/// <summary>
/// 覆盖 <c>AddSettings()</c> 开箱即用的装配（#310）。
/// </summary>
public class SettingsServiceCollectionExtensionsTests
{
    private sealed class TestSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            context.Add(
                new SettingDefinition("App.PageSize", defaultValue: "20"),
                new SettingDefinition("App.Theme", defaultValue: "light"));
        }
    }

    /// <summary>未在 DI 注册、仅登记在 options 中的自定义值提供者。</summary>
    private sealed class FixedSettingValueProvider : ISettingValueProvider
    {
        public string Name => "Fixed";

        public Task<string?> GetOrNullAsync(SettingDefinition setting)
            => Task.FromResult<string?>("fixed");

        public Task<List<SettingValue>> GetAllAsync(SettingDefinition[] settings)
            => Task.FromResult(settings.Select(s => new SettingValue(s.Name, "fixed")).ToList());
    }

    private static ServiceProvider BuildProvider(Action<IServiceCollection>? configure = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Settings:App.Theme"] = "dark" })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<ISettingDefinitionProvider, TestSettingDefinitionProvider>();
        services.AddSettings();
        configure?.Invoke(services);
        return services.BuildServiceProvider();
    }

    [Fact]
    public void AddSettings_RegistersBuiltInValueProvidersInPriorityOrder()
    {
        using var provider = BuildProvider();

        var options = provider.GetRequiredService<IOptions<SkywalkerSettingOptions>>().Value;

        Assert.Equal(
            new[] { typeof(ConfigurationSettingValueProvider), typeof(DefaultValueSettingValueProvider) },
            options.ValueProviders.ToArray());
    }

    [Fact]
    public async Task GetOrNullAsync_FallsBackToDefaultValue_WithoutManualProviderRegistration()
    {
        using var provider = BuildProvider();

        var settingProvider = provider.GetRequiredService<ISettingProvider>();

        Assert.Equal("20", await settingProvider.GetOrNullAsync("App.PageSize"));
    }

    [Fact]
    public async Task GetOrNullAsync_ConfigurationOverridesDefaultValue()
    {
        using var provider = BuildProvider();

        var settingProvider = provider.GetRequiredService<ISettingProvider>();

        Assert.Equal("dark", await settingProvider.GetOrNullAsync("App.Theme"));
    }

    [Fact]
    public async Task ValueProviderManager_ResolvesProviderNotRegisteredInDi()
    {
        using var provider = BuildProvider(services =>
            services.Configure<SkywalkerSettingOptions>(o => o.ValueProviders.Insert(0, typeof(FixedSettingValueProvider))));

        var settingProvider = provider.GetRequiredService<ISettingProvider>();

        Assert.Equal("fixed", await settingProvider.GetOrNullAsync("App.Theme"));
    }

    [Fact]
    public void AddSettings_UserConfigurationCanReplaceDefaults()
    {
        using var provider = BuildProvider(services =>
            services.Configure<SkywalkerSettingOptions>(o =>
            {
                o.ValueProviders.Clear();
                o.ValueProviders.Add<DefaultValueSettingValueProvider>();
            }));

        var options = provider.GetRequiredService<IOptions<SkywalkerSettingOptions>>().Value;

        Assert.Equal(new[] { typeof(DefaultValueSettingValueProvider) }, options.ValueProviders.ToArray());
    }
}
