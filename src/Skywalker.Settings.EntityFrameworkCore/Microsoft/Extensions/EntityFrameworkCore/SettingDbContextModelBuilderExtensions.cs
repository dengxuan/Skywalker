// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skywalker.Settings.EntityFrameworkCore;
using Skywalker.Settings.EntityFrameworkCore.Entities;

namespace Microsoft.Extensions.EntityFrameworkCore;

/// <summary>
/// <inheritdoc/>
/// </summary>
public static class SettingDbContextModelBuilderExtensions
{
    /// <summary>
    /// 配置ISettingDbContext的库,表关系
    /// </summary>
    /// <param name="builder"></param>
    public static void ConfigureSetting(this ModelBuilder builder, string dbTablePrefix = "", string? dbSchema = null)
    {
        builder.NotNull(nameof(builder));

        // Configure your own tables/entities inside this block

        // Configure Setting with composite primary key
        builder.Entity<Setting>(b =>
        {
            b.ToTable($"{dbTablePrefix}Settings", dbSchema);

            b.ConfigureByConvention();

            // Composite primary key: (Name, ProviderName, ProviderKey)
            b.HasKey(x => new { x.Name, x.ProviderName, x.ProviderKey });

            b.Property(x => x.Name).HasMaxLength(SettingConsts.MaxNameLength).IsRequired();
            b.Property(x => x.Value).HasMaxLength(SettingConsts.MaxValueLength).IsRequired();
            b.Property(x => x.ProviderName).HasMaxLength(SettingConsts.MaxProviderNameLength).IsRequired();
            b.Property(x => x.ProviderKey).HasMaxLength(SettingConsts.MaxProviderKeyLength).IsRequired();
        });
    }
}
