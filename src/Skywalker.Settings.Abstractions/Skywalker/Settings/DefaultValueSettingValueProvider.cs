using Skywalker.Settings.Abstractions;

namespace Skywalker.Settings;

/// <summary>
/// Setting value provider that returns the default value from the setting definition.
/// </summary>
public class DefaultValueSettingValueProvider : ISettingValueProvider
{
    public const string ProviderName = "D";

    public string Name => ProviderName;

    public Task<string?> GetOrNullAsync(SettingDefinition setting)
    {
        return Task.FromResult(setting.DefaultValue);
    }

    public Task<List<SettingValue>> GetAllAsync(SettingDefinition[] settings)
    {
        return Task.FromResult(settings.Select(x => new SettingValue(x.Name, x.DefaultValue)).ToList());
    }
}
