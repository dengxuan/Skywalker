namespace Skywalker.Settings.Abstractions;

public interface ISettingValueProvider
{
    string Name { get; }

    Task<string?> GetOrNullAsync(SettingDefinition setting);

    Task<List<SettingValue>> GetAllAsync(SettingDefinition[] settings);
}
