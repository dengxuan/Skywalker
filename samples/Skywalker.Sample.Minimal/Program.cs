using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Domain.Services;
using Skywalker.Ddd.EntityFrameworkCore;

var services = new ServiceCollection();
services.AddLogging();
services.AddSkywalkerDbContext<MinimalDbContext>(options =>
{
	options.Configure(context => context.DbContextOptions.UseInMemoryDatabase("Skywalker.Sample.Minimal"));
});

var generatedRegistration = typeof(MinimalDbContext).Assembly
	.GetCustomAttributes(typeof(SkywalkerGeneratedRepositoryRegistrationAttribute), inherit: false)
	.OfType<SkywalkerGeneratedRepositoryRegistrationAttribute>()
	.SingleOrDefault(attribute => attribute.DbContextType == typeof(MinimalDbContext));

Ensure(generatedRegistration is not null, "EF repository generator did not emit registration metadata.");
EnsureRegistered<IRepository<MinimalOrder, Guid>>(services);
EnsureRegistered<IRepository<MinimalAuditLog>>(services);
EnsureRegistered<IDomainService<MinimalOrder, Guid>>(services);
EnsureRegistered<IDomainService<MinimalAuditLog>>(services);

Console.WriteLine("Sample: Minimal — EF repository source generator registration verified.");

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

public sealed class MinimalDbContext(DbContextOptions<MinimalDbContext> options) : SkywalkerDbContext<MinimalDbContext>(options)
{
	public DbSet<MinimalOrder> Orders { get; set; } = default!;

	public DbSet<MinimalAuditLog> AuditLogs { get; set; } = default!;
}

public sealed class MinimalOrder : Entity<Guid>
{
}

public sealed class MinimalAuditLog : Entity
{
	public override object[] GetKeys() => [];
}
