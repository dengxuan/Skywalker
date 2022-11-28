// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public class DddBuilder
{
    /// <summary>
    /// 
    /// </summary>
    public IServiceCollection Services { get; }

    internal DddBuilder(IServiceCollection services) => Services = services;
}
