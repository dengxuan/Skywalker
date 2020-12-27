using Skywalker;
using Skywalker.Ddd.Infrastructure;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InfrastructureSkywalkerBuilderExtensions
    {
        public static SkywalkerBuilder AddInfrastructure(this SkywalkerBuilder builder, Action<SkywalkerRepositoryInitializer> optionsBuilder)
        {
            SkywalkerRepositoryInitializer initializer = new SkywalkerRepositoryInitializer(builder.Services);
            optionsBuilder?.Invoke(initializer);
            return builder;
        }
    }
}
