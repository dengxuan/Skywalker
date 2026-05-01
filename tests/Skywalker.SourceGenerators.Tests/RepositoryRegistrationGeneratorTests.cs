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

    [Fact]
    public void GeneratesDistinctRegistrars_ForMultipleDbContextsInSameAssembly()
    {
        var result = GeneratorTestHelper.Run<RepositoryRegistrationGenerator>("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : SkywalkerDbContext<CatalogDbContext>(options)
            {
                public DbSet<CatalogItem> Items { get; set; } = null!;
            }

            public sealed class BillingDbContext(DbContextOptions<BillingDbContext> options) : SkywalkerDbContext<BillingDbContext>(options)
            {
                public DbSet<BillingInvoice> Invoices { get; set; } = null!;
            }

            public sealed class CatalogItem : Entity<System.Guid>
            {
            }

            public sealed class BillingInvoice : Entity<long>
            {
            }
            """, CreateReferences());

        Assert.Equal(2, result.GeneratedTrees.Length);

        var generatedSources = result.GeneratedTrees
            .Select(tree => (tree.FilePath, Source: tree.GetText().ToString()))
            .ToArray();

        var catalogSource = Assert.Single(generatedSources, source => source.FilePath.EndsWith("Demo_CatalogDbContext.SkywalkerRepositoryRegistrations.g.cs"));
        var billingSource = Assert.Single(generatedSources, source => source.FilePath.EndsWith("Demo_BillingDbContext.SkywalkerRepositoryRegistrations.g.cs"));

        Assert.Contains("typeof(global::Demo.CatalogDbContext)", catalogSource.Source);
        Assert.Contains("typeof(global::Microsoft.Extensions.DependencyInjection.Demo_CatalogDbContextSkywalkerRepositoryRegistrations)", catalogSource.Source);
        Assert.Contains("nameof(global::Microsoft.Extensions.DependencyInjection.Demo_CatalogDbContextSkywalkerRepositoryRegistrations.AddSkywalkerGeneratedRepositoriesForDemo_CatalogDbContext)", catalogSource.Source);
        Assert.Contains("IRepository<global::Demo.CatalogItem, global::System.Guid>", catalogSource.Source);
        Assert.DoesNotContain("BillingInvoice", catalogSource.Source);

        Assert.Contains("typeof(global::Demo.BillingDbContext)", billingSource.Source);
        Assert.Contains("typeof(global::Microsoft.Extensions.DependencyInjection.Demo_BillingDbContextSkywalkerRepositoryRegistrations)", billingSource.Source);
        Assert.Contains("nameof(global::Microsoft.Extensions.DependencyInjection.Demo_BillingDbContextSkywalkerRepositoryRegistrations.AddSkywalkerGeneratedRepositoriesForDemo_BillingDbContext)", billingSource.Source);
        Assert.Contains("IRepository<global::Demo.BillingInvoice, long>", billingSource.Source);
        Assert.DoesNotContain("CatalogItem", billingSource.Source);
    }

    [Fact]
    public void ReportsDiagnostic_ForNonPublicDbSetProperty()
    {
        var result = GeneratorTestHelper.Run<RepositoryRegistrationGenerator>("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                internal DbSet<Order> Orders { get; set; } = null!;
            }

            public sealed class Order : Entity<int>
            {
            }
            """, CreateReferences());

        Assert.Empty(result.GeneratedTrees);
        AssertDiagnostic(
            result,
            "SKY3001",
            "DbSet property 'Orders' on DbContext 'Demo.SalesDbContext' must be a public instance property to participate in repository generation");
    }

    [Fact]
    public void ReportsDiagnostic_ForInaccessibleEntityType()
    {
        var result = GeneratorTestHelper.Run<RepositoryRegistrationGenerator>("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<PrivateOrder> Orders { get; set; } = null!;

                private sealed class PrivateOrder : Entity<int>
                {
                }
            }
            """, CreateReferences());

        Assert.Empty(result.GeneratedTrees);
        AssertDiagnostic(
            result,
            "SKY3002",
            "Entity type 'Demo.SalesDbContext.PrivateOrder' exposed by DbSet property 'Orders' must be public or internal so generated repository registrations can reference it");
    }

    [Fact]
    public void ReportsDiagnostic_ForEntityThatDoesNotImplementIEntity()
    {
        var result = GeneratorTestHelper.Run<RepositoryRegistrationGenerator>("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<PlainOrder> Orders { get; set; } = null!;
            }

            public sealed class PlainOrder
            {
            }
            """, CreateReferences());

        Assert.Empty(result.GeneratedTrees);
        AssertDiagnostic(
            result,
            "SKY3003",
            "Entity type 'Demo.PlainOrder' exposed by DbSet property 'Orders' must implement Skywalker.Ddd.Domain.Entities.IEntity");
    }

    [Fact]
    public void ReportsDiagnostic_ForAbstractEntityType()
    {
        var result = GeneratorTestHelper.Run<RepositoryRegistrationGenerator>("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<AbstractOrder> Orders { get; set; } = null!;
            }

            public abstract class AbstractOrder : Entity<int>
            {
            }
            """, CreateReferences());

        Assert.Empty(result.GeneratedTrees);
        AssertDiagnostic(
            result,
            "SKY3004",
            "Entity type 'Demo.AbstractOrder' exposed by DbSet property 'Orders' must be a non-abstract class");
    }

    [Fact]
    public void ReportsDiagnostic_ForDuplicateDbSetEntityRegistration()
    {
        var result = GeneratorTestHelper.Run<RepositoryRegistrationGenerator>("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<Order> Orders { get; set; } = null!;
                public DbSet<Order> AlsoOrders { get; set; } = null!;
            }

            public sealed class Order : Entity<int>
            {
            }
            """, CreateReferences());

        var generatedSource = Assert.Single(result.GeneratedTrees).GetText().ToString();
        Assert.Equal(1, CountOccurrences(generatedSource, "IRepository<global::Demo.Order, int>"));
        AssertDiagnostic(
            result,
            "SKY3005",
            "Entity type 'Demo.Order' is exposed by multiple DbSet properties on DbContext 'Demo.SalesDbContext': Orders, AlsoOrders");
    }

    [Fact]
    public void ReportsDiagnostic_ForConflictingEntityKeyTypes()
    {
        var result = GeneratorTestHelper.Run<RepositoryRegistrationGenerator>("""
            using System;
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<ConflictingOrder> Orders { get; set; } = null!;
            }

            public sealed class ConflictingOrder : Entity<int>, IEntity<Guid>
            {
                Guid IEntity<Guid>.Id => Guid.Empty;

                public bool Equals(Guid other) => false;
            }
            """, CreateReferences());

        Assert.Empty(result.GeneratedTrees);
        AssertDiagnostic(
            result,
            "SKY3006",
            "Entity type 'Demo.ConflictingOrder' implements IEntity<TKey> with conflicting key types: int, System.Guid");
    }

    private static void AssertDiagnostic(GeneratorDriverRunResult result, string id, string message)
    {
        var diagnostic = Assert.Single(result.Diagnostics, diagnostic => diagnostic.Id == id);
        Assert.Equal(DiagnosticSeverity.Warning, diagnostic.Severity);
        Assert.Equal(message, diagnostic.GetMessage());
    }

    private static int CountOccurrences(string value, string search)
    {
        var count = 0;
        var index = 0;
        while ((index = value.IndexOf(search, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += search.Length;
        }

        return count;
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