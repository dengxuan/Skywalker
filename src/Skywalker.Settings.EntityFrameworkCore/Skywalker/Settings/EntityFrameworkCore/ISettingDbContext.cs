// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Settings.EntityFrameworkCore.Entities;

namespace Skywalker.Settings.EntityFrameworkCore;

public interface ISettingDbContext : ISkywalkerDbContext
{
    public DbSet<Setting> Settings { get; set; }
}
