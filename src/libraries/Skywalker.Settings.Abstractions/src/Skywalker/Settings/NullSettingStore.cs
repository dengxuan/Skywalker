﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Skywalker.Settings.Abstractions;

namespace Skywalker.Settings;

//[Dependency(TryRegister = true)]
public class NullSettingStore : ISettingStore//, ISingletonDependency
{
    public ILogger<NullSettingStore> Logger { get; set; }

    public NullSettingStore()
    {
        Logger = NullLogger<NullSettingStore>.Instance;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="providerName"></param>
    /// <param name="providerKey"></param>
    /// <returns></returns>
    public Task<string?> GetOrNullAsync(string name, string? providerName, string? providerKey)
    {
        return Task.FromResult<string?>(null);
    }

    public Task<List<SettingValue>> GetAllAsync(string[] names, string? providerName, string? providerKey)
    {
        return Task.FromResult(names.Select(x => new SettingValue(x, null)).ToList());
    }
}
