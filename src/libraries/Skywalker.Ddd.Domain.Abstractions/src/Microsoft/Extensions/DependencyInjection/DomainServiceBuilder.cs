// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Data;
using Skywalker.Ddd.Data.Filtering;
using Skywalker.Ddd.Data.Seeding;
using Skywalker.Ddd.Domain.Entities.Events.Distributed;
using Skywalker.Ddd.Domain.Services;

namespace Microsoft.Extensions.DependencyInjection;

public class DomainServiceBuilder
{
    public IServiceCollection Services { get; set; }

    internal DomainServiceBuilder(IServiceCollection services) => Services = services;

    public DomainServiceBuilder AddDomainServicesCore()
    {
        var configuration = Services.GetConfiguration();
        Services.Configure<SkywalkerDbConnectionOptions>(configuration);
        Services.AddSingleton<IDataSeeder, DataSeeder>();
        Services.AddTransient<IConnectionStringResolver, DefaultConnectionStringResolver>();
        Services.AddTransient<IEntityToEtoMapper, EntityToEtoMapper>();
        Services.AddSingleton(typeof(IDataFilter<>), typeof(DataFilter<>));
        Services.AddTransient(typeof(IDomainService<>), typeof(DomainService<>));
        Services.AddTransient(typeof(IDomainService<,>), typeof(DomainService<,>));
        return this;
    }
}
