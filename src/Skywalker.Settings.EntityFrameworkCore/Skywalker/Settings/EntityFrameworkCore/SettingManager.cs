// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Extensions.Specifications;
using Skywalker.Settings.Abstractions;
using Skywalker.Settings.EntityFrameworkCore.Entities;
using Skywalker.Settings.EntityFrameworkCore.Specifications;

namespace Skywalker.Settings.EntityFrameworkCore;

/// <summary>
/// Entity Framework Core implementation of <see cref="ISettingManager"/>.
/// Uses repository pattern for database operations.
/// </summary>
public class SettingManager(
    IRepository<Setting> repository,
    ISettingDefinitionManager settingDefinitionManager,
    ISettingEncryptionService settingEncryptionService) : ISettingManager
{
    /// <inheritdoc />
    public virtual async Task SetAsync(string name, string value, string providerName, string? providerKey = null, CancellationToken cancellationToken = default)
    {
        // Validate that the setting is defined
        var definition = settingDefinitionManager.GetOrNull(name) ?? throw new ArgumentException($"Setting '{name}' is not defined.", nameof(name));

        // Encrypt value if the setting is marked as encrypted
        var valueToStore = definition.IsEncrypted 
            ? settingEncryptionService.Encrypt(value) 
            : value;

        ISpecification<Setting> specification = new SettingSpecification(name, providerName, providerKey);
        var setting = await repository.FindAsync(specification, cancellationToken);

        if (setting == null)
        {
            setting = new Setting(name, valueToStore!, providerName, providerKey);
            await repository.InsertAsync(setting, autoSave: true, cancellationToken);
        }
        else
        {
            setting.SetValue(valueToStore!);
            await repository.UpdateAsync(setting, autoSave: true, cancellationToken);
        }
    }

    /// <inheritdoc />
    public virtual async Task DeleteAsync(string name, string providerName, string? providerKey = null, CancellationToken cancellationToken = default)
    {
        ISpecification<Setting> specification = new SettingSpecification(name, providerName, providerKey);
        var setting = await repository.FindAsync(specification, cancellationToken);

        if (setting != null)
        {
            await repository.DeleteAsync(setting, autoSave: true, cancellationToken);
        }
    }
}
