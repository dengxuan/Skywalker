// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Extensions.DependencyInjection;

public class Skywalking
{
    internal IServiceCollection Services { get; }

    internal Skywalking(IServiceCollection services) => Services = services;
}
