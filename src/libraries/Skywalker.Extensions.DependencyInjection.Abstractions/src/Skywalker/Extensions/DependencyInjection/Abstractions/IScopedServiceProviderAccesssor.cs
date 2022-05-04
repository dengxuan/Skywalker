// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.DependencyInjection.Abstractions;

/// <summary>
/// Represents accessor to get current ambient scoped service provider.
/// </summary>
public interface IScopedServiceProviderAccesssor
{
    /// <summary>
    /// Gets current ambient scoped service provider.
    /// </summary>
    IServiceProvider Current { get; }
}
