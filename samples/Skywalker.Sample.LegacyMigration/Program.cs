using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Domain.Services;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.Ddd.Uow.EntityFrameworkCore;
using Skywalker.EventBus.Abstractions;
using Skywalker.Extensions.Timezone;

var services = new ServiceCollection();
services.AddLogging();
services.AddTransient<IRepository<LegacyOrder, Guid>, LegacyOrderRepository>();
services.AddSkywalkerDbContext<LegacyDbContext>(options =>
{
	options.Configure(context => context.DbContextOptions.UseInMemoryDatabase("Skywalker.Sample.LegacyMigration"));
});

var generatedRegistration = typeof(LegacyDbContext).Assembly
	.GetCustomAttributes(typeof(SkywalkerGeneratedRepositoryRegistrationAttribute), inherit: false)
	.OfType<SkywalkerGeneratedRepositoryRegistrationAttribute>()
	.SingleOrDefault(attribute => attribute.DbContextType == typeof(LegacyDbContext));

Ensure(generatedRegistration is not null, "EF repository generator did not emit registration metadata.");
EnsureSingleRegistration<IRepository<LegacyOrder, Guid>, LegacyOrderRepository>(services);
EnsureRegistered<IRepository<LegacyOrder>>(services);
EnsureRegistered<IDomainService<LegacyOrder, Guid>>(services);
EnsureRegistered<IDomainService<LegacyOrder>>(services);

Console.WriteLine("Sample: LegacyMigration — manual repository registration coexists with generated defaults.");

static void EnsureSingleRegistration<TService, TImplementation>(IServiceCollection services)
{
	var descriptors = services.Where(descriptor => descriptor.ServiceType == typeof(TService)).ToArray();
	Ensure(descriptors.Length == 1, $"Expected exactly one registration for {typeof(TService).FullName}, found {descriptors.Length}.");
	Ensure(descriptors[0].ImplementationType == typeof(TImplementation), $"Expected legacy implementation {typeof(TImplementation).FullName} for {typeof(TService).FullName}.");
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

public sealed class LegacyDbContext(DbContextOptions<LegacyDbContext> options) : SkywalkerDbContext<LegacyDbContext>(options)
{
	public DbSet<LegacyOrder> Orders { get; set; } = default!;
}

public sealed class LegacyOrder : Entity<Guid>
{
}

public sealed class LegacyOrderRepository(
	IClock clock,
	IEventBus eventBus,
	IDbContextProvider<LegacyDbContext> dbContextProvider,
	IUnitOfWorkAccessor unitOfWorkAccessor)
	: Repository<LegacyDbContext, LegacyOrder, Guid>(clock, eventBus, dbContextProvider, unitOfWorkAccessor);
