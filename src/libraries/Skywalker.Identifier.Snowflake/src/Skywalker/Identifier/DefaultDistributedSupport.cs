// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.Options;

namespace Skywalker.Identifier;

internal class DefaultDistributedSupport : IDistributedSupport
{
    private readonly IdentifierGeneratorOptions _options;

    public DefaultDistributedSupport(IOptions<IdentifierGeneratorOptions> options)
    {
        _options = options.Value;
    }

    public Task<ushort> GetNextMechineId() => Task.FromResult(_options.WorkId);

    public Task RefreshAlive() => Task.CompletedTask;
}
