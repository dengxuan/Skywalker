// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Extensions.Specifications;
using Skywalker.Settings.Abstractions;
using Skywalker.Settings.EntityFrameworkCore.Entities;
using Skywalker.Settings.EntityFrameworkCore.Specifications;

namespace Skywalker.Settings.EntityFrameworkCore;

/// <summary>
/// Base class for setting value providers that use the repository pattern.
/// </summary>
public abstract class EfCoreSettingValueProvider(IRepository<Setting> repository) : ISettingValueProvider
{
    /// <summary>
    /// Gets the Setting repository.
    /// </summary>
    protected IRepository<Setting> Repository { get; } = repository;

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <summary>
    /// Gets the provider key for the current context (e.g., user ID, tenant ID).
    /// </summary>
    protected abstract string? GetProviderKey();

    /// <inheritdoc />
    public virtual async Task<string?> GetOrNullAsync(SettingDefinition setting)
    {
        ISpecification<Setting> specification = new SettingSpecification(setting.Name, Name, GetProviderKey());
        var entity = await Repository.FindAsync(specification);
        return entity?.Value;
    }

    /// <inheritdoc />
    public virtual async Task<List<SettingValue>> GetAllAsync(SettingDefinition[] settings)
    {
        var names = settings.Select(x => x.Name).ToArray();
        ISpecification<Setting> specification = new SettingsSpecification(names, Name, GetProviderKey());
        var entities = await Repository.GetListAsync(specification);
        return entities.Select(s => new SettingValue(s.Name, s.Value)).ToList();
    }
}
