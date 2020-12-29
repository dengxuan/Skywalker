using Skywalker.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration
{
    public class SkywalkerDbContextConfigurerAction : ISkywalkerDbContextConfigurer
    {
        [NotNull]
        public Action<SkywalkerDbContextConfigurationContext> Action { get; }

        public SkywalkerDbContextConfigurerAction([NotNull] Action<SkywalkerDbContextConfigurationContext> action)
        {
            Check.NotNull(action, nameof(action));

            Action = action;
        }

        public void Configure(SkywalkerDbContextConfigurationContext context)
        {
            Action.Invoke(context);
        }
    }

    public class SkywalkerDbContextConfigurerAction<TDbContext> : SkywalkerDbContextConfigurerAction where TDbContext : ISkywalkerDbContext
    {
        public SkywalkerDbContextConfigurerAction([NotNull] Action<SkywalkerDbContextConfigurationContext> action)
            : base(action)
        {
        }
    }
}