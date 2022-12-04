// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.Options;

namespace Skywalker.Extensions.Snowflake;

internal class DefaultWorker : IWorker
{
    private readonly SnowflakeGeneratorOptions _options;

    public DefaultWorker(IOptions<SnowflakeGeneratorOptions> options)
    {
        _options = options.Value;
    }

    public ushort NextWorkerId() => _options.WorkerId;

    public Task RefreshAlive() => Task.CompletedTask;
}
