using Microsoft.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Sample.Website.Domain;

namespace Skywalker.Sample.Website.EntityFrameworkCore;

/// <summary>
/// 站点 DbContext：EF SG 在编译期扫描 DbSet 生成 IRepository / IDomainService 注册。
/// </summary>
public class WebsiteDbContext(DbContextOptions<WebsiteDbContext> options) : SkywalkerDbContext<WebsiteDbContext>(options)
{
    public DbSet<Member> Members { get; set; } = default!;
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<Order> Orders { get; set; } = default!;
    public DbSet<OrderItem> OrderItems { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(order =>
        {
            order.HasMany(o => o.Items).WithOne().HasForeignKey(i => i.OrderId);
            order.Navigation(o => o.Items).AutoInclude();
        });
    }
}
