using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration
{
    public class SkywalkerDbContextConfigurerAction : ISkywalkerDbContextConfigurer
    {
        
        public Action<SkywalkerDbContextConfigurationContext> Action { get; }

        public SkywalkerDbContextConfigurerAction(Action<SkywalkerDbContextConfigurationContext> action)
        {
            Check.NotNull(action, nameof(action));

            Action = action;
        }

        public void Configure(SkywalkerDbContextConfigurationContext context)
        {
            Action.Invoke(context);
        }
    }

    public class SkywalkerDbContextConfigurerAction<TDbContext> : SkywalkerDbContextConfigurerAction where TDbContext : DbContext
    {
        public SkywalkerDbContextConfigurerAction(Action<SkywalkerDbContextConfigurationContext> action)
            : base(action)
        {
        }
    }
}