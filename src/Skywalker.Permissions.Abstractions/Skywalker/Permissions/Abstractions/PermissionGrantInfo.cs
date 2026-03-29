// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace Skywalker.Permissions.Abstractions;

/// <summary>
/// 权限授权信息
/// </summary>
public class PermissionGrantInfo
{
    public string Name { get; }

    public bool IsGranted { get; }

    public string? ProviderName { get; }

    public string? ProviderKey { get; }

    [JsonConstructor]
    public PermissionGrantInfo(string name, bool isGranted, string? providerName = null, string? providerKey = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        Name = name;
        IsGranted = isGranted;
        ProviderName = providerName;
        ProviderKey = providerKey;
    }
}
