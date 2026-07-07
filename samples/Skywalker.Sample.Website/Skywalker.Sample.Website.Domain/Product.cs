using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Sample.Website.Domain;

/// <summary>
/// 商品实体：演示 EF SG 自动仓储注册（IRepository&lt;Product, Guid&gt; 无需手写）。
/// </summary>
public class Product : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    protected Product() { }

    public Product(Guid id, string name, decimal price, int stock) : base(id)
    {
        Name = name;
        Price = price;
        Stock = stock;
    }
}
