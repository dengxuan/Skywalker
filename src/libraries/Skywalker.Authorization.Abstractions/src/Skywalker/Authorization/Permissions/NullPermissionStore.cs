﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Skywalker.Authorization.Permissions.Abstractions;
using Skywalker.Extensions.Threading;

namespace Skywalker.Authorization.Permissions;

public class NullPermissionStore : IPermissionStore
{
    public ILogger<NullPermissionStore> Logger { get; set; }

    public NullPermissionStore()
    {
        Logger = NullLogger<NullPermissionStore>.Instance;
    }

    public Task<bool> IsGrantedAsync(string name, string? providerName, string? providerKey)
    {
        //Todo: TaskCache.FalseResult;
        return Task.FromResult(false);
        //return TaskCache.FalseResult;
    }

    public Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names, string? providerName, string? providerKey)
    {
        return Task.FromResult(new MultiplePermissionGrantResult(names, PermissionGrantResult.Prohibited));
    }
}
