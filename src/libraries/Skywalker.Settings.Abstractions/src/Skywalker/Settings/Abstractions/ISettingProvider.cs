namespace Skywalker.Settings.Abstractions;

public interface ISettingProvider
{
    Task<string> GetOrNullAsync(string name);

    Task<List<SettingValue>> GetAllAsync(string[] names);

    Task<List<SettingValue>> GetAllAsync();
}
