// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Settings.EntityFrameworkCore.Entities;

namespace Skywalker.Settings.EntityFrameworkCore;

public class SettingDbContext(DbContextOptions<SettingDbContext> options) : SkywalkerDbContext<SettingDbContext>(options), ISettingDbContext
{
    public virtual DbSet<Setting> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureSetting();
    }
}
