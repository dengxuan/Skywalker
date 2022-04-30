using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Identifier.Abstractions;

/// <summary>
/// Used to generate Identifier.
/// </summary>
public interface IIdentifierGenerator<TIdentifier> : ISingletonDependency where TIdentifier : notnull
{
    TIdentifier Create();
}
