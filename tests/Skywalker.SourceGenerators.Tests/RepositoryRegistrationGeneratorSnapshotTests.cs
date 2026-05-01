using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.SourceGenerators;

namespace Skywalker.SourceGenerators.Tests;

public sealed class RepositoryRegistrationGeneratorSnapshotTests
{
    [Fact]
    public Task SingleDbContext_WithGuidKey()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<Order> Orders { get; set; } = null!;
            }

            public sealed class Order : Entity<System.Guid>
            {
            }
            """);

    [Fact]
    public Task SingleDbContext_WithIntKey()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<Order> Orders { get; set; } = null!;
            }

            public sealed class Order : Entity<int>
            {
            }
            """);

    [Fact]
    public Task SingleDbContext_WithLongKey()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class BillingDbContext(DbContextOptions<BillingDbContext> options) : SkywalkerDbContext<BillingDbContext>(options)
            {
                public DbSet<Invoice> Invoices { get; set; } = null!;
            }

            public sealed class Invoice : Entity<long>
            {
            }
            """);

    [Fact]
    public Task SingleDbContext_WithStringKey()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class TenantDbContext(DbContextOptions<TenantDbContext> options) : SkywalkerDbContext<TenantDbContext>(options)
            {
                public DbSet<Tenant> Tenants { get; set; } = null!;
            }

            public sealed class Tenant : Entity<string>
            {
            }
            """);

    [Fact]
    public Task SingleDbContext_WithNonKeyedEntity()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class AuditDbContext(DbContextOptions<AuditDbContext> options) : SkywalkerDbContext<AuditDbContext>(options)
            {
                public DbSet<AuditLog> AuditLogs { get; set; } = null!;
            }

            public sealed class AuditLog : Entity
            {
                public override object[] GetKeys() => [];
            }
            """);

    [Fact]
    public Task SingleDbContext_WithTwoEntities()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<Order> Orders { get; set; } = null!;
                public DbSet<Customer> Customers { get; set; } = null!;
            }

            public sealed class Order : Entity<int>
            {
            }

            public sealed class Customer : Entity<System.Guid>
            {
            }
            """);

    [Fact]
    public Task MultipleDbContexts_InSameNamespace()
        => VerifyRepositorySnapshot("""
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
                public DbSet<Invoice> Invoices { get; set; } = null!;
            }

            public sealed class CatalogItem : Entity<System.Guid>
            {
            }

            public sealed class Invoice : Entity<long>
            {
            }
            """);

    [Fact]
    public Task MultipleDbContexts_InDifferentNamespaces()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo.Catalog;

            public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : SkywalkerDbContext<CatalogDbContext>(options)
            {
                public DbSet<CatalogItem> Items { get; set; } = null!;
            }

            public sealed class CatalogItem : Entity<System.Guid>
            {
            }

            namespace Demo.Billing;

            public sealed class BillingDbContext(DbContextOptions<BillingDbContext> options) : SkywalkerDbContext<BillingDbContext>(options)
            {
                public DbSet<Invoice> Invoices { get; set; } = null!;
            }

            public sealed class Invoice : Entity<long>
            {
            }
            """);

    [Fact]
    public Task EmptyDbContext_ProducesNoSources()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class EmptyDbContext(DbContextOptions<EmptyDbContext> options) : SkywalkerDbContext<EmptyDbContext>(options)
            {
            }
            """);

    [Fact]
    public Task DbContext_WithOnlyIgnoredNonPublicDbSet()
        => VerifyRepositorySnapshot("""
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
            """);

    [Fact]
    public Task DbContext_WithValidAndIgnoredNonPublicDbSet()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<Order> Orders { get; set; } = null!;
                internal DbSet<HiddenOrder> HiddenOrders { get; set; } = null!;
            }

            public sealed class Order : Entity<int>
            {
            }

            public sealed class HiddenOrder : Entity<int>
            {
            }
            """);

    [Fact]
    public Task DbContext_WithPlainEfEntityOnly()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<PlainOrder> Orders { get; set; } = null!;
            }

            public sealed class PlainOrder
            {
                public int Id { get; set; }
            }
            """);

    [Fact]
    public Task DbContext_WithValidAndPlainEfEntity()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<Order> Orders { get; set; } = null!;
                public DbSet<PlainOrder> PlainOrders { get; set; } = null!;
            }

            public sealed class Order : Entity<int>
            {
            }

            public sealed class PlainOrder
            {
                public int Id { get; set; }
            }
            """);

    [Fact]
    public Task DbContext_WithAbstractEntityOnly()
        => VerifyRepositorySnapshot("""
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
            """);

    [Fact]
    public Task DbContext_WithDuplicateEntityDbSets()
        => VerifyRepositorySnapshot("""
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
            """);

    [Fact]
    public Task DbContext_WithConflictingKeyTypes()
        => VerifyRepositorySnapshot("""
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
            """);

    [Fact]
    public Task InternalEntity_IsGenerated()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<Order> Orders { get; set; } = null!;
            }

            internal sealed class Order : Entity<int>
            {
            }
            """);

    [Fact]
    public Task PrivateNestedEntity_ProducesNoSources()
        => VerifyRepositorySnapshot("""
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
            """);

    [Fact]
    public Task PublicNestedEntity_IsGenerated()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<Order> Orders { get; set; } = null!;

                public sealed class Order : Entity<int>
                {
                }
            }
            """);

    [Fact]
    public Task DeepNestedEntity_IsGenerated()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<Outer.Inner.Order> Orders { get; set; } = null!;
            }

            public static class Outer
            {
                public static class Inner
                {
                    public sealed class Order : Entity<int>
                    {
                    }
                }
            }
            """);

    [Fact]
    public Task BlockScopedNamespace_IsGenerated()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo
            {
                public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
                {
                    public DbSet<Order> Orders { get; set; } = null!;
                }

                public sealed class Order : Entity<int>
                {
                }
            }
            """);

    [Fact]
    public Task GlobalNamespace_IsGenerated()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<Order> Orders { get; set; } = null!;
            }

            public sealed class Order : Entity<int>
            {
            }
            """);

    [Fact]
    public Task UsingAlias_ForDbSet_IsGenerated()
        => VerifyRepositorySnapshot("""
            using EfDbSet = Microsoft.EntityFrameworkCore.DbSet<Demo.Order>;
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public EfDbSet Orders { get; set; } = null!;
            }

            public sealed class Order : Entity<int>
            {
            }
            """);

    [Fact]
    public Task SameShortEntityName_InDifferentNamespaces()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : SkywalkerDbContext<AppDbContext>(options)
            {
                public DbSet<Catalog.Order> CatalogOrders { get; set; } = null!;
                public DbSet<Billing.Order> BillingOrders { get; set; } = null!;
            }

            namespace Demo.Catalog;

            public sealed class Order : Entity<int>
            {
            }

            namespace Demo.Billing;

            public sealed class Order : Entity<long>
            {
            }
            """);

    [Fact]
    public Task DbContextNameCollision_WithGeneratedHelperSuffix()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContextSkywalkerRepositoryRegistrations(DbContextOptions<SalesDbContextSkywalkerRepositoryRegistrations> options) : SkywalkerDbContext<SalesDbContextSkywalkerRepositoryRegistrations>(options)
            {
                public DbSet<Order> Orders { get; set; } = null!;
            }

            public sealed class Order : Entity<int>
            {
            }
            """);

    [Fact]
    public Task DerivedEntityBaseClass_InSameAssembly()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<Order> Orders { get; set; } = null!;
            }

            public abstract class AuditedEntity<TKey> : Entity<TKey>
                where TKey : notnull
            {
            }

            public sealed class Order : AuditedEntity<int>
            {
            }
            """);

    [Fact]
    public Task ExplicitIEntityImplementation_IsGenerated()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<Order> Orders { get; set; } = null!;
            }

            public sealed class Order : IEntity<int>
            {
                public int Id { get; set; }

                object[] IEntity.GetKeys() => [Id];

                public bool Equals(int other) => Id == other;
            }
            """);

    [Fact]
    public Task GenericEntityClosedByDerivedClass_IsGenerated()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<Order> Orders { get; set; } = null!;
            }

            public abstract class EntityBase<TKey> : Entity<TKey>
                where TKey : notnull
            {
            }

            public sealed class Order : EntityBase<System.Guid>
            {
            }
            """);

    [Fact]
    public Task DbContext_WithManualServiceRegistrationCompilesGeneratedDefaults()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Microsoft.Extensions.DependencyInjection;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.Domain.Repositories;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public DbSet<Order> Orders { get; set; } = null!;
            }

            public sealed class Order : Entity<int>
            {
            }

            public sealed class CustomOrderRepository : IRepository<Order, int>
            {
            }

            public static class ManualRegistration
            {
                public static IServiceCollection AddManualRepositories(this IServiceCollection services)
                    => services.AddTransient<IRepository<Order, int>, CustomOrderRepository>();
            }
            """);

    [Fact]
    public Task DeterministicOrdering_FollowsDbSetDeclarationOrder()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : SkywalkerDbContext<AppDbContext>(options)
            {
                public DbSet<ThirdEntity> Third { get; set; } = null!;
                public DbSet<FirstEntity> First { get; set; } = null!;
                public DbSet<SecondEntity> Second { get; set; } = null!;
            }

            public sealed class FirstEntity : Entity<int>
            {
            }

            public sealed class SecondEntity : Entity<int>
            {
            }

            public sealed class ThirdEntity : Entity<int>
            {
            }
            """);

    [Fact]
    public Task UnsupportedStaticDbSet_ProducesNoSources()
        => VerifyRepositorySnapshot("""
            using Microsoft.EntityFrameworkCore;
            using Skywalker.Ddd.Domain.Entities;
            using Skywalker.Ddd.EntityFrameworkCore;

            namespace Demo;

            public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
            {
                public static DbSet<Order> Orders { get; set; } = null!;
            }

            public sealed class Order : Entity<int>
            {
            }
            """);

    private static Task VerifyRepositorySnapshot(string source)
    {
        var result = GeneratorTestHelper.Run<RepositoryRegistrationGenerator>(source, CreateReferences());
        var builder = new StringBuilder();

        foreach (var diagnostic in result.Diagnostics.OrderBy(diagnostic => diagnostic.Id).ThenBy(diagnostic => diagnostic.GetMessage()))
        {
            builder.Append("Diagnostic ")
                .Append(diagnostic.Id)
                .Append(" [")
                .Append(diagnostic.Severity)
                .Append("]: ")
                .AppendLine(diagnostic.GetMessage());
        }

        if (result.GeneratedTrees.Length == 0)
        {
            builder.AppendLine("<no generated sources>");
        }
        else
        {
            foreach (var tree in result.GeneratedTrees.OrderBy(tree => NormalizePath(tree.FilePath), StringComparer.Ordinal))
            {
                builder.Append("// File: ").AppendLine(NormalizePath(tree.FilePath));
                builder.AppendLine(tree.GetText().ToString().TrimEnd());
                builder.AppendLine();
            }
        }

        return Verifier.Verify(builder.ToString());
    }

    private static string NormalizePath(string path) => path.Replace('\\', '/');

    private static IEnumerable<MetadataReference> CreateReferences()
    {
        yield return MetadataReference.CreateFromFile(typeof(DbContext).Assembly.Location);
        yield return MetadataReference.CreateFromFile(typeof(DbSet<>).Assembly.Location);
        yield return MetadataReference.CreateFromFile(typeof(IServiceCollection).Assembly.Location);
        yield return MetadataReference.CreateFromFile(typeof(Entity).Assembly.Location);
        yield return MetadataReference.CreateFromFile(typeof(SkywalkerDbContext<>).Assembly.Location);
    }
}