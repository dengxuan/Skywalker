using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Identifier.Abstractions;

/// <summary>
/// Used to generate Identifier.
/// </summary>
public interface IIdentifier : ISingletonDependency
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    TIdentifier? GetIdentifier<TIdentifier>() where TIdentifier : notnull;
}
