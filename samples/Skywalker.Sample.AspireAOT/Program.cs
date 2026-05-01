using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Domain.Services;
using Skywalker.Identity.Domain.Repositories;
using Skywalker.Sample.AspireAOT;

var services = new ServiceCollection();
global::Microsoft.Extensions.DependencyInjection.Skywalker_Sample_AspireAOT_AotDbContextSkywalkerRepositoryRegistrations.AddSkywalkerGeneratedRepositoriesForSkywalker_Sample_AspireAOT_AotDbContext(services);

EnsureRegistered<IRepository<AotOrder, Guid>, Repository<AotDbContext, AotOrder, Guid>>(services);
EnsureRegistered<IRepository<AotOrder>, Repository<AotDbContext, AotOrder, Guid>>(services);
EnsureRegistered<IDomainService<AotOrder, Guid>, EntityFrameworkCoreDomainService<AotOrder, Guid>>(services);
EnsureRegistered<IDomainService<AotOrder>, EntityFrameworkCoreDomainService<AotOrder, Guid>>(services);

Console.WriteLine("Sample: AspireAOT — EF repository source generator registration verified.");

static void EnsureRegistered<TService, TImplementation>(IServiceCollection services)
{
	Ensure(
		services.Any(descriptor => descriptor.ServiceType == typeof(TService) && descriptor.ImplementationType == typeof(TImplementation)),
		$"Missing service registration: {typeof(TService).FullName}");
}

static void Ensure(bool condition, string message)
{
	if (!condition)
	{
		throw new InvalidOperationException(message);
	}
}

namespace Skywalker.Sample.AspireAOT
{
	public sealed class AotDbContext : Skywalker.Ddd.EntityFrameworkCore.SkywalkerDbContext<AotDbContext>
	{
		public Microsoft.EntityFrameworkCore.DbSet<AotOrder> Orders { get; set; } = default!;
	}

	public sealed class AotOrder : Skywalker.Ddd.Domain.Entities.Entity<Guid>
	{
	}
}

namespace Microsoft.EntityFrameworkCore
{
	public sealed class DbSet<TEntity>
		where TEntity : class
	{
	}
}

namespace Skywalker.Ddd.EntityFrameworkCore
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class SkywalkerGeneratedRepositoryRegistrationAttribute(Type dbContextType, Type registrarType, string methodName) : Attribute
	{
		public Type DbContextType { get; } = dbContextType;

		public Type RegistrarType { get; } = registrarType;

		public string MethodName { get; } = methodName;
	}

	public abstract class SkywalkerDbContext<TDbContext>
		where TDbContext : SkywalkerDbContext<TDbContext>
	{
	}
}

namespace Skywalker.Ddd.Domain.Entities
{
	public interface IEntity
	{
	}

	public interface IEntity<TKey> : IEntity
	{
	}

	public abstract class Entity<TKey> : IEntity<TKey>
	{
	}
}

namespace Skywalker.Ddd.Domain.Repositories
{
	public interface IReadOnlyRepository<TEntity>
		where TEntity : class, Skywalker.Ddd.Domain.Entities.IEntity
	{
	}

	public interface IReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity>
		where TEntity : class, Skywalker.Ddd.Domain.Entities.IEntity<TKey>
	{
	}

	public interface IBasicRepository<TEntity> : IReadOnlyRepository<TEntity>
		where TEntity : class, Skywalker.Ddd.Domain.Entities.IEntity
	{
	}

	public interface IBasicRepository<TEntity, TKey> : IBasicRepository<TEntity>, IReadOnlyRepository<TEntity, TKey>
		where TEntity : class, Skywalker.Ddd.Domain.Entities.IEntity<TKey>
	{
	}

	public interface IRepository<TEntity> : IBasicRepository<TEntity>
		where TEntity : class, Skywalker.Ddd.Domain.Entities.IEntity
	{
	}

	public interface IRepository<TEntity, TKey> : IRepository<TEntity>, IBasicRepository<TEntity, TKey>
		where TEntity : class, Skywalker.Ddd.Domain.Entities.IEntity<TKey>
	{
	}

	public sealed class Repository<TDbContext, TEntity, TKey> : IRepository<TEntity, TKey>
		where TEntity : class, Skywalker.Ddd.Domain.Entities.IEntity<TKey>
	{
	}
}

namespace Skywalker.Ddd.Domain.Services
{
	public interface IDomainService<TEntity>
		where TEntity : class, Skywalker.Ddd.Domain.Entities.IEntity
	{
	}

	public interface IDomainService<TEntity, TKey> : IDomainService<TEntity>
		where TEntity : class, Skywalker.Ddd.Domain.Entities.IEntity<TKey>
	{
	}
}

namespace Skywalker.Identity.Domain.Repositories
{
	public sealed class EntityFrameworkCoreDomainService<TEntity, TKey> : Skywalker.Ddd.Domain.Services.IDomainService<TEntity, TKey>
		where TEntity : class, Skywalker.Ddd.Domain.Entities.IEntity<TKey>
	{
	}
}
