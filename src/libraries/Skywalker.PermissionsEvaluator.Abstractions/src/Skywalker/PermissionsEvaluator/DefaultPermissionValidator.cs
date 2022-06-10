using Microsoft.Extensions.Logging;

/* Unmerged change from project 'Skywalker.PermissionsEvaluator.Abstractions (netstandard2.1)'
Before:
using Skywalker.PermissionsEvaluator.Abstractions;
After:
using Skywalker;
using Skywalker.PermissionsEvaluator;
using Skywalker.PermissionsEvaluator;
using Skywalker.PermissionsEvaluator.Permissions;
using Skywalker.PermissionsEvaluator.Abstractions;
*/
using Skywalker.PermissionsEvaluator.Abstractions;

namespace Skywalker.PermissionsEvaluator;

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
