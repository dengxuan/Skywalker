using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Infrastructure.DbContextConfiguration;
using Skywalker.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.Infrastructure
{
    public class SkywalkerDbContextOptions
    {
        internal List<Action<SkywalkerDbContextConfigurationContext>> DefaultPreConfigureActions { get; set; }

        internal Action<SkywalkerDbContextConfigurationContext>? DefaultConfigureAction { get; set; }

        internal Dictionary<Type, List<object>> PreConfigureActions { get; set; }

        internal Dictionary<Type, object> ConfigureActions { get; set; }

        public SkywalkerDbContextOptions()
        {
            DefaultPreConfigureActions = new List<Action<SkywalkerDbContextConfigurationContext>>();
            PreConfigureActions = new Dictionary<Type, List<object>>();
            ConfigureActions = new Dictionary<Type, object>();
        }

        public void PreConfigure([NotNull] Action<SkywalkerDbContextConfigurationContext> action)
        {
            Check.NotNull(action, nameof(action));

            DefaultPreConfigureActions.Add(action);
        }

        public void Configure([NotNull] Action<SkywalkerDbContextConfigurationContext> action)
        {
            Check.NotNull(action, nameof(action));

            DefaultConfigureAction = action;
        }

        public void PreConfigure<TDbContext>([NotNull] Action<SkywalkerDbContextConfigurationContext<TDbContext>> action) where TDbContext : SkywalkerDbContext<TDbContext>
        {
            Check.NotNull(action, nameof(action));

            var actions = PreConfigureActions.GetOrDefault(typeof(TDbContext));
            if (actions == null)
            {
                PreConfigureActions[typeof(TDbContext)] = actions = new List<object>();
            }

            actions.Add(action);
        }

        public void Configure<TDbContext>([NotNull] Action<SkywalkerDbContextConfigurationContext<TDbContext>> action) where TDbContext : SkywalkerDbContext<TDbContext>
        {
            Check.NotNull(action, nameof(action));

            ConfigureActions[typeof(TDbContext)] = action;
        }
    }
}