using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Domain.Services;
using Skywalker.Ddd.EntityFrameworkCore;

var services = new ServiceCollection();
services.AddLogging();
services.AddSkywalkerDbContext<Commerce.SalesDbContext>(options =>
{
	options.Configure(context => context.DbContextOptions.UseInMemoryDatabase("Skywalker.Sample.NestedTypes"));
});

var generatedRegistration = typeof(Commerce.SalesDbContext).Assembly
	.GetCustomAttributes(typeof(SkywalkerGeneratedRepositoryRegistrationAttribute), inherit: false)
	.OfType<SkywalkerGeneratedRepositoryRegistrationAttribute>()
	.SingleOrDefault(attribute => attribute.DbContextType == typeof(Commerce.SalesDbContext));

Ensure(generatedRegistration is not null, "EF repository generator did not emit registration metadata for the nested DbContext.");
EnsureRegistered<IRepository<Commerce.Order, Guid>>(services);
EnsureRegistered<IDomainService<Commerce.Order, Guid>>(services);
EnsureRegistered<IRepository<Commerce.Catalog.Item, int>>(services);
EnsureRegistered<IDomainService<Commerce.Catalog.Item, int>>(services);

Console.WriteLine("Sample: NestedTypes - EF repository generator handles nested DbContext and entity types.");

static void EnsureRegistered<TService>(IServiceCollection services)
{
	Ensure(services.Any(descriptor => descriptor.ServiceType == typeof(TService)), $"Missing service registration: {typeof(TService).FullName}");
}

static void Ensure(bool condition, string message)
{
	if (!condition)
	{
		throw new InvalidOperationException(message);
	}
}

public static partial class Commerce
{
	public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : SkywalkerDbContext<SalesDbContext>(options)
	{
		public DbSet<Order> Orders { get; set; } = default!;

		public DbSet<Catalog.Item> CatalogItems { get; set; } = default!;
	}

	public sealed class Order : Entity<Guid>
	{
	}

	public static partial class Catalog
	{
		public sealed class Item : Entity<int>
		{
		}
	}
}
