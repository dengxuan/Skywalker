// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.CodeDom.Compiler;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.Repositories;
using Skywalker.Ddd.EntityFrameworkCoreTest.Domain.Entities;
using Skywalker.Extensions.Timezone;
using Skywalker.Identifier.Abstractions;
using Skywalker.Ddd.Domain.Services;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#nullable disable
namespace Skywalker.Ddd.EntityFrameworkCoreTest;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkCore(this IServiceCollection services)
    {
        return services;
    }
}

public record class Test : IEntity
{
    public object[] GetKeys() => new[] { "1", "2" };
}

public class TestA : IEntity<long>
{
    public long Id { get; set; }

    public bool Equals(long other) => Id == other;
    public object[] GetKeys() => new object[] { Id };
}

public class TestDbContext : SkywalkerDbContext<TestDbContext>
{
    public DbSet<User> Users { get; set; }
    //public DbSet<Username> UserNames { get; set; }

    //public DbSet<Schoole> Schooles { get; set; }

    //public DbSet<Test> Tests { get; set; }
    //public DbSet<TestA> TestAs { get; set; }

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    base.OnModelCreating(modelBuilder);
    //    modelBuilder.Entity<User>(b =>
    //    {
    //        b.ToTable("User");
    //        b.ConfigureByConvention();
    //    });
    //}
}
#nullable enable
