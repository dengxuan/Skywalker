// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions;

public class DefaultPermissionValidator : IPermissionValidator
{
    private readonly ILogger<DefaultPermissionValidator> _logger;

    public DefaultPermissionValidator(ILogger<DefaultPermissionValidator> logger)
    {
        _logger = logger;
    }

    public Task<bool> IsGrantedAsync(string name, string? providerName, string? providerKey)
    {
        _logger.LogInformation("{name} {providerName} {providerKey} is prohibited", name, providerName, providerKey);
        return TaskCache.FalseResult;
    }

    public Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names, string? providerName, string? providerKey)
    {
        var permissions = names.JoinAsString(",");
        _logger.LogInformation("{permissions} {providerName} {providerKey} is prohibited", permissions, providerName, providerKey);
        return Task.FromResult(new MultiplePermissionGrantResult(names, PermissionGrantResult.Prohibited));
    }
}
