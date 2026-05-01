using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Domain.Services;
using Skywalker.Ddd.EntityFrameworkCore;

var services = new ServiceCollection();
services.AddLogging();
services.AddSkywalkerDbContext<CatalogDbContext>(options =>
{
	options.Configure(context => context.DbContextOptions.UseInMemoryDatabase("Skywalker.Sample.MultiDbContext.Catalog"));
});
services.AddSkywalkerDbContext<BillingDbContext>(options =>
{
	options.Configure(context => context.DbContextOptions.UseInMemoryDatabase("Skywalker.Sample.MultiDbContext.Billing"));
});

var generatedRegistrations = typeof(CatalogDbContext).Assembly
	.GetCustomAttributes(typeof(SkywalkerGeneratedRepositoryRegistrationAttribute), inherit: false)
	.OfType<SkywalkerGeneratedRepositoryRegistrationAttribute>()
	.ToArray();

var catalogRegistration = EnsureGeneratedRegistration<CatalogDbContext>(generatedRegistrations);
var billingRegistration = EnsureGeneratedRegistration<BillingDbContext>(generatedRegistrations);

Ensure(catalogRegistration.RegistrarType != billingRegistration.RegistrarType, "Generated registrars for two DbContexts should be distinct.");
EnsureRegistered<IRepository<CatalogItem, Guid>>(services);
EnsureRegistered<IDomainService<CatalogItem, Guid>>(services);
EnsureRegistered<IRepository<BillingInvoice, long>>(services);
EnsureRegistered<IDomainService<BillingInvoice, long>>(services);

Console.WriteLine("Sample: MultiDbContext — EF repository generator handles two DbContexts.");

static SkywalkerGeneratedRepositoryRegistrationAttribute EnsureGeneratedRegistration<TDbContext>(SkywalkerGeneratedRepositoryRegistrationAttribute[] registrations)
	where TDbContext : DbContext
{
	var registration = registrations.SingleOrDefault(attribute => attribute.DbContextType == typeof(TDbContext));
	Ensure(registration is not null, $"EF repository generator did not emit registration metadata for {typeof(TDbContext).FullName}.");

	return registration!;
}

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

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : SkywalkerDbContext<CatalogDbContext>(options)
{
	public DbSet<CatalogItem> Items { get; set; } = default!;
}

public sealed class BillingDbContext(DbContextOptions<BillingDbContext> options) : SkywalkerDbContext<BillingDbContext>(options)
{
	public DbSet<BillingInvoice> Invoices { get; set; } = default!;
}

public sealed class CatalogItem : Entity<Guid>
{
}

public sealed class BillingInvoice : Entity<long>
{
}
