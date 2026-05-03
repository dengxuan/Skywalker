using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Domain.Services;
using Skywalker.Extensions.DynamicProxies;
using Skywalker.Identity.Domain.Repositories;
using Skywalker.Sample.AspireAOT;

var services = new ServiceCollection();
global::Microsoft.Extensions.DependencyInjection.Skywalker_Sample_AspireAOT_AotDbContextSkywalkerRepositoryRegistrations.AddSkywalkerGeneratedRepositoriesForSkywalker_Sample_AspireAOT_AotDbContext(services);
services.AddSingleton<CallRecorder>();
services.AddSingleton<IInterceptor, RecordingInterceptor>();
services.AddSingleton<AotOrderService>();
services.AddSingleton<AotStringLookupService>();

EnsureRegistered<IRepository<AotOrder, Guid>, Repository<AotDbContext, AotOrder, Guid>>(services);
EnsureRegistered<IRepository<AotOrder>, Repository<AotDbContext, AotOrder, Guid>>(services);
EnsureRegistered<IDomainService<AotOrder, Guid>, EntityFrameworkCoreDomainService<AotOrder, Guid>>(services);
EnsureRegistered<IDomainService<AotOrder>, EntityFrameworkCoreDomainService<AotOrder, Guid>>(services);

using var provider = services.BuildServiceProvider();
var interceptors = provider.GetServices<IInterceptor>();
var orderProxy = new global__Skywalker_Sample_AspireAOT_AotOrderService_global__Skywalker_Sample_AspireAOT_IAotOrderServiceSkywalkerProxy(
	provider.GetRequiredService<AotOrderService>(),
	interceptors);
var lookupProxy = new global__Skywalker_Sample_AspireAOT_AotStringLookupService_global__Skywalker_Sample_AspireAOT_IAotLookupService_stringSkywalkerProxy(
	provider.GetRequiredService<AotStringLookupService>(),
	interceptors);

var orderResult = await orderProxy.SubmitAsync("AOT-001");
var lookupResult = lookupProxy.Echo("edge");
var recorder = provider.GetRequiredService<CallRecorder>();

Ensure(orderResult == "submitted:AOT-001", "DynamicProxy generated async proxy returned an unexpected value.");
Ensure(lookupResult == "edge", "DynamicProxy generated closed generic proxy returned an unexpected value.");
Ensure(
	recorder.Entries.SequenceEqual(new[]
	{
		"before:SubmitAsync:AOT-001",
		"after:SubmitAsync:submitted:AOT-001",
		"before:Echo:edge",
		"after:Echo:edge",
	}),
	"DynamicProxy generated proxy interceptor order was not preserved.");

Console.WriteLine("Sample: AspireAOT — EF repository and DynamicProxy source generator paths verified.");

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

	public interface IAotOrderService : Skywalker.Extensions.DynamicProxies.IInterceptable
	{
		Task<string> SubmitAsync(string number);
	}

	public sealed class AotOrderService : IAotOrderService
	{
		public Task<string> SubmitAsync(string number)
		{
			return Task.FromResult($"submitted:{number}");
		}
	}

	public interface IAotLookupService<T> : Skywalker.Extensions.DynamicProxies.IInterceptable
	{
		T Echo(T value);
	}

	public sealed class AotStringLookupService : IAotLookupService<string>
	{
		public string Echo(string value) => value;
	}

	public sealed class CallRecorder
	{
		private readonly List<string> _entries = [];

		public IReadOnlyList<string> Entries => _entries;

		public void Add(string entry)
		{
			_entries.Add(entry);
		}
	}

	public sealed class RecordingInterceptor(CallRecorder recorder) : Skywalker.Extensions.DynamicProxies.IInterceptor
	{
		public async Task InterceptAsync(Skywalker.Extensions.DynamicProxies.IMethodInvocation invocation)
		{
			recorder.Add($"before:{invocation.MethodName}:{invocation.Arguments[0]}");
			await invocation.ProceedAsync().ConfigureAwait(false);
			recorder.Add($"after:{invocation.MethodName}:{invocation.ReturnValue}");
		}
	}
}

namespace Skywalker.Extensions.DynamicProxies
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class SkywalkerGeneratedDynamicProxyAttribute(Type serviceType, Type implementationType, Type proxyType) : Attribute
	{
		public Type ServiceType { get; } = serviceType;

		public Type ImplementationType { get; } = implementationType;

		public Type ProxyType { get; } = proxyType;
	}

	public interface IInterceptable
	{
	}

	public interface IInterceptor
	{
		Task InterceptAsync(IMethodInvocation invocation);
	}

	public interface IMethodInvocation
	{
		object Target { get; }

		System.Reflection.MethodInfo Method { get; }

		string MethodName { get; }

		object?[] Arguments { get; }

		Type ReturnType { get; }

		object? ReturnValue { get; set; }

		Task ProceedAsync();
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
