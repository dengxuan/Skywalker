// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.Repositories;
#nullable disable
namespace Skywalker.Ddd.EntityFrameworkCoreTest;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkCore(this IServiceCollection services)
    {
        services.TryAddTransient<IRepository<User>, Repository<TestDbContext, User>>();
        services.TryAddTransient<IRepository<User, Guid>, Repository<TestDbContext, User, Guid>>();

        services.TryAddTransient<IBasicRepository<User>, Repository<TestDbContext, User>>();
        services.TryAddTransient<IBasicRepository<User, Guid>, Repository<TestDbContext, User, Guid>>();

        services.TryAddTransient<IReadOnlyRepository<User>, Repository<TestDbContext, User>>();
        services.TryAddTransient<IReadOnlyRepository<User, Guid>, Repository<TestDbContext, User, Guid>>();
        return services;
    }
}

public class TestDbContext : SkywalkerDbContext<TestDbContext>
{
    public DbSet<User> Users { get; set; }
    public DbSet<Username> UserNames { get; set; }

    public DbSet<Schoole> Schooles { get; set; }

    public DbSet<Test> Tests { get; set; }
    public DbSet<TestA> TestAs { get; set; }

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }
}
#nullable enable
