// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions.AspNetCore;

/// <summary>
/// 空实现，不返回任何授权数据
/// </summary>
public class NullPermissionGrantStore : IPermissionGrantStore
{
    public Task<IReadOnlyList<PermissionGrantInfo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<PermissionGrantInfo>>(Array.Empty<PermissionGrantInfo>());
    }

    public Task<string> GetVersionAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(string.Empty);
    }
}
