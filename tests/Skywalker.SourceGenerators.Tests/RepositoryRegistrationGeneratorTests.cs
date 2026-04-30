using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.SourceGenerators;

namespace Skywalker.SourceGenerators.Tests;

public sealed class RepositoryRegistrationGeneratorTests
{
    [Fact]
    public void GeneratesRepositoryRegistrations_ForDbSetEntities()
    {
        var result = GeneratorTestHelper.Run<RepositoryRegistrationGenerator>("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<Order> Orders { get; set; } = null!;
                public DbSet<AuditLog> AuditLogs { get; set; } = null!;
                internal DbSet<IgnoredEntity> IgnoredEntities { get; set; } = null!;
            }

            public sealed class Order : Entity<int>
            {
            }

            public sealed class AuditLog : Entity
            {
                public override object[] GetKeys() => [];
            }

            public sealed class IgnoredEntity : Entity<int>
            {
            }
            """, CreateReferences());

        var generatedTree = Assert.Single(result.GeneratedTrees);
        var generatedSource = generatedTree.GetText().ToString();

        Assert.EndsWith("Demo_SalesDbContext.SkywalkerRepositoryRegistrations.g.cs", generatedTree.FilePath);
        Assert.Contains("[assembly: global::Skywalker.Ddd.EntityFrameworkCore.SkywalkerGeneratedRepositoryRegistrationAttribute", generatedSource);
        Assert.Contains("AddSkywalkerGeneratedRepositoriesForDemo_SalesDbContext", generatedSource);
        Assert.Contains("IRepository<global::Demo.Order, int>", generatedSource);
        Assert.Contains("Repository<global::Demo.SalesDbContext, global::Demo.Order, int>", generatedSource);
        Assert.Contains("IDomainService<global::Demo.Order, int>", generatedSource);
        Assert.Contains("IRepository<global::Demo.AuditLog>", generatedSource);
        Assert.Contains("Repository<global::Demo.SalesDbContext, global::Demo.AuditLog>", generatedSource);
        Assert.DoesNotContain("IgnoredEntity", generatedSource);
    }

    [Fact]
    public void SkipsTypesWithoutDbSetEntities()
    {
        var result = GeneratorTestHelper.Run<RepositoryRegistrationGenerator>("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class EmptyDbContext(DbContextOptions<EmptyDbContext> options) : SkywalkerDbContext<EmptyDbContext>(options)
            {
            }
            """, CreateReferences());

        Assert.Empty(result.GeneratedTrees);
    }

    private static IEnumerable<MetadataReference> CreateReferences()
    {
        yield return MetadataReference.CreateFromFile(typeof(DbContext).Assembly.Location);
        yield return MetadataReference.CreateFromFile(typeof(DbSet<>).Assembly.Location);
        yield return MetadataReference.CreateFromFile(typeof(IServiceCollection).Assembly.Location);
        yield return MetadataReference.CreateFromFile(typeof(Entity).Assembly.Location);
        yield return MetadataReference.CreateFromFile(typeof(SkywalkerDbContext<>).Assembly.Location);
    }
}