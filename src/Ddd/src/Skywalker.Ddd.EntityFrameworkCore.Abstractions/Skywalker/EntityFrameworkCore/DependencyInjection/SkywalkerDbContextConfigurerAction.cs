using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.EntityFrameworkCore.DependencyInjection
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

    public class AbpDbContextConfigurerAction<TDbContext> : SkywalkerDbContextConfigurerAction
        where TDbContext : SkywalkerDbContext<TDbContext>
    {
        public AbpDbContextConfigurerAction([NotNull] Action<SkywalkerDbContextConfigurationContext> action) 
            : base(action)
        {
        }
    }
}