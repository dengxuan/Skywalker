using Microsoft.Extensions.DependencyInjection;
using System;

namespace Skywalker.Ddd.Infrastructure
{
    public class SkywalkerRepositoryInitializer
    {
        internal IServiceCollection Services { get; }

        public SkywalkerRepositoryInitializer(IServiceCollection services)
        {
            Services = services;
        }

        internal void AddRepositories<TDatabase>()
        {
            SkywalkerDbContextRegistrationOptions options = new SkywalkerDbContextRegistrationOptions(typeof(TDatabase), Services);
            options.AddDefaultRepositories();
            SkywalkerRepositoryRegistrar registrar = new SkywalkerRepositoryRegistrar(options);
            registrar.AddRepositories();
        }
    }
}
