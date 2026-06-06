using Microsoft.Extensions.DependencyInjection;
using Skywalker.DependencyInjection;

var services = new ServiceCollection();
services.AddLogging();
services.AddSkywalker(typeof(Program).Assembly);

EnsureRegistered<IInternalGreetingService, InternalGreetingService>(services, ServiceLifetime.Scoped);

using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();

var greetingService = scope.ServiceProvider.GetRequiredService<IInternalGreetingService>();
Ensure(greetingService.GetGreeting() == "hello from internal service", "Generated DI registration resolved the wrong service implementation.");

Console.WriteLine("Sample: InternalServices - DI source generator registered an internal application service.");

static void EnsureRegistered<TService, TImplementation>(IServiceCollection services, ServiceLifetime lifetime)
{
	Ensure(
		services.Any(descriptor =>
			descriptor.ServiceType == typeof(TService) &&
			descriptor.ImplementationType == typeof(TImplementation) &&
			descriptor.Lifetime == lifetime),
		$"Missing generated service registration: {typeof(TService).FullName} -> {typeof(TImplementation).FullName} ({lifetime}).");
}

static void Ensure(bool condition, string message)
{
	if (!condition)
	{
		throw new InvalidOperationException(message);
	}
}

internal interface IInternalGreetingService
{
	string GetGreeting();
}

[ApplicationService]
internal sealed class InternalGreetingService : IInternalGreetingService
{
	public string GetGreeting() => "hello from internal service";
}
