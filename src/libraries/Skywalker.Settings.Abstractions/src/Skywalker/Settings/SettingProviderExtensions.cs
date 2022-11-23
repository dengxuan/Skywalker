using Skywalker.Settings.Abstractions;

namespace Skywalker.Settings;

public static class SettingProviderExtensions
{
    public static async Task<bool> IsTrueAsync(this ISettingProvider settingProvider, string name)
    {
        settingProvider.NotNull(nameof(settingProvider));
        name.NotNull(nameof(name));

        return string.Equals(
            await settingProvider.GetOrNullAsync(name),
            "true",
            StringComparison.OrdinalIgnoreCase
        );
    }

    public static async Task<T> GetAsync<T>(this ISettingProvider settingProvider, string name, T defaultValue = default)
        where T : struct
    {
        settingProvider.NotNull(nameof(settingProvider));
        name.NotNull(nameof(name));

        var value = await settingProvider.GetOrNullAsync(name);
        return value?.To<T>() ?? defaultValue;
    }
}
