using Microsoft.EntityFrameworkCore;
using Skywalker;
using Skywalker.Ddd.Infrastructure;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Ddd.Infrastructure.EntityFrameworkCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InfrastructureSkywalkerBuilderExtensions
    {
        public static SkywalkerRepositoryInitializer AddEntityFrameworkCore<TDbContext>(this SkywalkerRepositoryInitializer builder, Action<SkywalkerDbContextOptions> optionsBuilder) where TDbContext : DbContext
        {
            builder.Services.Configure(optionsBuilder);
            builder.Services.AddTransient(typeof(ISkywalkerDatabase<>), typeof(SkywalkerEntityFrameworkCoreDatabase<,>));
            builder.Services.AddTransient(typeof(ISkywalkerDatabase<,>), typeof(SkywalkerEntityFrameworkCoreDatabase<,,>));
            builder.Services.AddDbContext<TDbContext>();
            builder.AddRepositories<TDbContext>();
            return builder;
        }
    }
}
