// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Microsoft.Extensions.DependencyInjection;

public class SkywalkerDbContextBuilder
{
    public IServiceCollection Services { get; set; }

    internal SkywalkerDbContextBuilder(IServiceCollection services) => Services = services;
}
