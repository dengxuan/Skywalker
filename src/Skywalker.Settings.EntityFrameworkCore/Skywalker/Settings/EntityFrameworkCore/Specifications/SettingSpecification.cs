// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Linq.Expressions;
using Skywalker.Extensions.Specifications;
using Skywalker.Settings.EntityFrameworkCore.Entities;

namespace Skywalker.Settings.EntityFrameworkCore.Specifications;

/// <summary>
/// Specification for querying settings by provider.
/// </summary>
public class SettingSpecification : Specification<Setting>
{
    private readonly string _name;
    private readonly string _providerName;
    private readonly string _providerKey;

    /// <summary>
    /// Creates a specification for a single setting.
    /// </summary>
    public SettingSpecification(string name, string providerName, string? providerKey)
    {
        _name = name;
        _providerName = providerName;
        _providerKey = providerKey ?? string.Empty;
    }

    /// <inheritdoc />
    public override Expression<Func<Setting, bool>> ToExpression()
    {
        return s => s.Name == _name && s.ProviderName == _providerName && s.ProviderKey == _providerKey;
    }
}

/// <summary>
/// Specification for querying multiple settings by provider.
/// </summary>
public class SettingsSpecification : Specification<Setting>
{
    private readonly string[] _names;
    private readonly string _providerName;
    private readonly string _providerKey;

    /// <summary>
    /// Creates a specification for multiple settings.
    /// </summary>
    public SettingsSpecification(string[] names, string providerName, string? providerKey)
    {
        _names = names;
        _providerName = providerName;
        _providerKey = providerKey ?? string.Empty;
    }

    /// <inheritdoc />
    public override Expression<Func<Setting, bool>> ToExpression()
    {
        return s => _names.Contains(s.Name) && s.ProviderName == _providerName && s.ProviderKey == _providerKey;
    }
}
