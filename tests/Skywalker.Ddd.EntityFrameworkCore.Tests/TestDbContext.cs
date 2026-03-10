using Microsoft.EntityFrameworkCore;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Ddd.EntityFrameworkCore.Tests;

/// <summary>
/// 测试用实体
/// </summary>
public class TestEntity : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }

    public TestEntity() { }
    public TestEntity(Guid id) : base(id) { }
}

/// <summary>
/// 带软删除的测试实体
/// </summary>
public class SoftDeleteEntity : Entity<Guid>, IDeletable
{
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletionTime { get; set; }

    public SoftDeleteEntity() { }
    public SoftDeleteEntity(Guid id) : base(id) { }
}

/// <summary>
/// 测试用 DbContext
/// </summary>
public class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities => Set<TestEntity>();
    public DbSet<SoftDeleteEntity> SoftDeleteEntities => Set<SoftDeleteEntity>();

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestEntity>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<SoftDeleteEntity>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.Name).HasMaxLength(200);
        });
    }
}

