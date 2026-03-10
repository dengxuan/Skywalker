// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Security.Users;
using Skywalker.Settings.EntityFrameworkCore.Entities;

namespace Skywalker.Settings.EntityFrameworkCore;

/// <summary>
/// User setting value provider that reads user-specific settings from database.
/// </summary>
public class UserSettingValueProvider(IRepository<Setting> repository, ICurrentUser currentUser)
    : EfCoreSettingValueProvider(repository)
{
    /// <summary>
    /// Provider name constant.
    /// </summary>
    public const string ProviderName = "U";

    /// <inheritdoc />
    public override string Name => ProviderName;

    /// <inheritdoc />
    protected override string? GetProviderKey() => currentUser.Id;
}
