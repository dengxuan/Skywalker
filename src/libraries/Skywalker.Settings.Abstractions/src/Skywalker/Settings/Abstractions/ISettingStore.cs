namespace Skywalker.Settings.Abstractions;

public interface ISettingStore
{
    Task<string?> GetOrNullAsync(string name, string? providerName, string? providerKey);

    Task<List<SettingValue>> GetAllAsync(string[] names, string? providerName, string? providerKey);
}
