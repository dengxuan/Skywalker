using Skywalker.Settings.Abstractions;

namespace Skywalker.Settings;

public class SettingProvider(
    ISettingDefinitionManager settingDefinitionManager,
    ISettingEncryptionService settingEncryptionService,
    ISettingValueProviderManager settingValueProviderManager) : ISettingProvider
{
    protected ISettingDefinitionManager SettingDefinitionManager { get; } = settingDefinitionManager;

    protected ISettingEncryptionService SettingEncryptionService { get; } = settingEncryptionService;

    protected ISettingValueProviderManager SettingValueProviderManager { get; } = settingValueProviderManager;

    public virtual async Task<string> GetAsync(string name)
    {
        var value = await GetOrNullAsync(name) ?? throw new KeyNotFoundException(name);
        return value;
    }

    public virtual async Task<string?> GetOrNullAsync(string name)
    {
        var setting = SettingDefinitionManager.GetOrNull(name);
        if(setting == null)
        {
            return null;
        }
        
        // Providers are already ordered by priority in Options (highest priority first)
        var providers = SettingValueProviderManager.Providers.AsEnumerable();

        if (setting.Providers.Count != 0)
        {
            providers = providers.Where(p => setting.Providers.Contains(p.Name));
        }

        //TODO: How to implement setting.IsInherited?

        var value = await GetOrNullValueFromProvidersAsync(providers, setting);
        if (!value.IsNullOrEmpty() && setting.IsEncrypted)
        {
            value = SettingEncryptionService.Decrypt(value);
        }

        return value;
    }

    public virtual async Task<List<SettingValue>> GetAllAsync(string[] names)
    {
        var result = new Dictionary<string, SettingValue>();
        var settingDefinitions = SettingDefinitionManager.GetAll().Where(x => names.Contains(x.Name)).ToList();

        foreach (var definition in settingDefinitions)
        {
            result.Add(definition.Name, new SettingValue(definition.Name, null));
        }

        // Providers are already ordered by priority in Options (highest priority first)
        foreach (var provider in SettingValueProviderManager.Providers)
        {
            var settingValues = await provider.GetAllAsync(settingDefinitions.Where(x => x.Providers.Count == 0 || x.Providers.Contains(provider.Name)).ToArray());

            var notNullValues = settingValues.Where(x => x.Value != null).ToList();
            foreach (var settingValue in notNullValues)
            {
                var settingDefinition = settingDefinitions.First(x => x.Name == settingValue.Name);
                if (settingDefinition.IsEncrypted)
                {
                    settingValue.Value = SettingEncryptionService.Decrypt(settingValue.Value);
                }

                if (result.TryGetValue(settingValue.Name, out var value) && value.Value == null)
                {
                    value.Value = settingValue.Value;
                }
            }

            settingDefinitions.RemoveAll(x => notNullValues.Any(v => v.Name == x.Name));
            if (settingDefinitions.Count == 0)
            {
                break;
            }
        }

        return [.. result.Values];
    }

    public virtual async Task<List<SettingValue>> GetAllAsync()
    {
        var settingValues = new List<SettingValue>();
        var settingDefinitions = SettingDefinitionManager.GetAll();

        foreach (var setting in settingDefinitions)
        {
            settingValues.Add(new SettingValue(setting.Name, await GetOrNullAsync(setting.Name)));
        }

        return settingValues;
    }

    protected virtual async Task<string?> GetOrNullValueFromProvidersAsync(IEnumerable<ISettingValueProvider> providers, SettingDefinition setting)
    {
        foreach (var provider in providers)
        {
            var value = await provider.GetOrNullAsync(setting);
            if (value != null)
            {
                return value;
            }
        }

        return null;
    }
}
