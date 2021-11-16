using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.Data;
using Skywalker.Ddd.ExceptionHandling;
using System;
using System.Collections.Generic;

namespace Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration
{
    public static class SkywalkerDbContextOptionsFactory
    {
        public static DbContextOptions<TDbContext> Create<TDbContext>(IServiceProvider serviceProvider)
            where TDbContext : SkywalkerDbContext<TDbContext>
        {
            var creationContext = GetCreationContext<TDbContext>(serviceProvider);

            var context = new SkywalkerDbContextConfigurationContext<TDbContext>(
                creationContext.ConnectionString!,
                serviceProvider,
                creationContext.ConnectionStringName,
                creationContext.ExistingConnection
            );

            var options = GetDbContextOptions<TDbContext>(serviceProvider);

            PreConfigure(options, context);
            Configure(options, context);

            return context.DbContextOptions.Options;
        }

        private static void PreConfigure<TDbContext>(SkywalkerDbContextOptions options,
            SkywalkerDbContextConfigurationContext<TDbContext> context)
            where TDbContext : SkywalkerDbContext<TDbContext>
        {
            foreach (var defaultPreConfigureAction in options.DefaultPreConfigureActions)
            {
                defaultPreConfigureAction.Invoke(context);
            }

            var preConfigureActions = options.PreConfigureActions.GetOrDefault(typeof(TDbContext));
            if (!preConfigureActions.IsNullOrEmpty())
            {
                foreach (var preConfigureAction in preConfigureActions!)
                {
                    ((Action<SkywalkerDbContextConfigurationContext<TDbContext>>)preConfigureAction).Invoke(context);
                }
            }
        }

        private static void Configure<TDbContext>(SkywalkerDbContextOptions options,
            SkywalkerDbContextConfigurationContext<TDbContext> context)
            where TDbContext : SkywalkerDbContext<TDbContext>
        {
            var configureAction = options.ConfigureActions.GetOrDefault(typeof(TDbContext));
            if (configureAction != null)
            {
                ((Action<SkywalkerDbContextConfigurationContext<TDbContext>>)configureAction).Invoke(context);
            }
            else if (options.DefaultConfigureAction != null)
            {
                options.DefaultConfigureAction.Invoke(context);
            }
            else
            {
                throw new SkywalkerException($"No configuration found for {typeof(DbContext).AssemblyQualifiedName}! Use services.Configure<SkywalkerDbContextOptions>(...) to configure it.");
            }
        }

        private static SkywalkerDbContextOptions GetDbContextOptions<TDbContext>(IServiceProvider serviceProvider)
            where TDbContext : SkywalkerDbContext<TDbContext>
        {
            return serviceProvider.GetRequiredService<IOptions<SkywalkerDbContextOptions>>().Value;
        }

        private static SkywalkerDbContextCreationContext GetCreationContext<TDbContext>(IServiceProvider serviceProvider)
            where TDbContext : SkywalkerDbContext<TDbContext>
        {
            var context = SkywalkerDbContextCreationContext.Current;
            if (context != null)
            {
                return context;
            }

            var connectionStringName = ConnectionStringNameAttribute.GetConnStringName<TDbContext>();
            var connectionString = serviceProvider.GetRequiredService<IConnectionStringResolver>().Resolve(connectionStringName);

            return new SkywalkerDbContextCreationContext(
                connectionStringName,
                connectionString
            );
        }
    }
}
