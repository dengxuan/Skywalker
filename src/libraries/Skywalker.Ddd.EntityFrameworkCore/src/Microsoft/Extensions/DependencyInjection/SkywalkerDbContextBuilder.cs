// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;

namespace Microsoft.Extensions.DependencyInjection;

public class SkywalkerDbContextBuilder
{
    public IServiceCollection Services { get; set; }

    internal SkywalkerDbContextBuilder(IServiceCollection services) => Services = services;

    public SkywalkerDbContextBuilder AddDbContextFactory<TDbContext>() where TDbContext : SkywalkerDbContext<TDbContext>
    {
        Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
#if NETSTANDARD2_0
        Services.AddDbContext<TDbContext>();
#else
        Services.AddDbContextFactory<TDbContext>();
#endif
        return this;
    }

    public SkywalkerDbContextBuilder AddDbContext<TDbContext>() where TDbContext : SkywalkerDbContext<TDbContext>
    {
        Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
#if NETSTANDARD2_0
        Services.AddDbContext<TDbContext>();
#else
        Services.AddDbContextFactory<TDbContext>();
#endif
        return this;
    }

}
