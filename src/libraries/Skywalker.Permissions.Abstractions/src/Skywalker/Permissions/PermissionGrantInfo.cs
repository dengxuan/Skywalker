﻿// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Permissions;

public class PermissionGrantInfo
{
    public string Name { get; }

    public bool IsGranted { get; }

    public string? ProviderName { get; }

    public string? ProviderKey { get; }

    public PermissionGrantInfo(string name, bool isGranted, string? providerName = null, string? providerKey = null)
    {
        name.NotNull(nameof(name));

        Name = name;
        IsGranted = isGranted;
        ProviderName = providerName;
        ProviderKey = providerKey;
    }
}
