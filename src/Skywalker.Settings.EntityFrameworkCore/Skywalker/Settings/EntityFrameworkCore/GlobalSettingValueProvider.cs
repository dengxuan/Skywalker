// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Settings.EntityFrameworkCore.Entities;

namespace Skywalker.Settings.EntityFrameworkCore;

/// <summary>
/// Global setting value provider that reads from database.
/// </summary>
public class GlobalSettingValueProvider(IRepository<Setting> repository)
    : EfCoreSettingValueProvider(repository)
{
    /// <summary>
    /// Provider name constant.
    /// </summary>
    public const string ProviderName = "G";

    /// <inheritdoc />
    public override string Name => ProviderName;

    /// <inheritdoc />
    protected override string? GetProviderKey() => null;
}
