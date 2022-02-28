// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.CodeDom.Compiler;
using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.Repositories;
using Skywalker.Ddd.EntityFrameworkCoreTest.Domain.Entities;
using Skywalker.Extensions.GuidGenerator;
using Skywalker.Extensions.Timing;
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

public record class Test : IEntity
{
    public object[] GetKeys() => new[] { "1", "2" };
}

public record class TestA : IEntity<long>
{
    public long Id { get; set; }

    public object[] GetKeys() => new object[] { Id };
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
