﻿using Microsoft.Extensions.Configuration;
using Skywalker.Settings.Abstractions;

namespace Skywalker.Settings;

public class ConfigurationSettingValueProvider : ISettingValueProvider//, ITransientDependency
{
    public const string ConfigurationNamePrefix = "Settings:";

    public const string ProviderName = "C";

    public string Name => ProviderName;

    protected IConfiguration Configuration { get; }

    public ConfigurationSettingValueProvider(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public virtual Task<string?> GetOrNullAsync(SettingDefinition setting)
    {
        return Task.FromResult(Configuration[ConfigurationNamePrefix + setting.Name]);
    }

    public Task<List<SettingValue>> GetAllAsync(SettingDefinition[] settings)
    {
        return Task.FromResult(settings.Select(x => new SettingValue(x.Name, Configuration[ConfigurationNamePrefix + x.Name])).ToList());
    }
}
