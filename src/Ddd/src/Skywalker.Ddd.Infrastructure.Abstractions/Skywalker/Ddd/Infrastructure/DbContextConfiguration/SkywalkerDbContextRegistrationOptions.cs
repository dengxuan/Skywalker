using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.DependencyInjection;
using Skywalker.Domain.Entities;

namespace Skywalker.Ddd.Infrastructure.DbContextConfiguration
{
    public class SkywalkerDbContextRegistrationOptions : SkywalkerCommonDbContextRegistrationOptions, ISkywalkerDbContextRegistrationOptionsBuilder
    {
        public Dictionary<Type, object> EntityOptions { get; }

        public SkywalkerDbContextRegistrationOptions(Type originalDbContextType, IServiceCollection services) : base(originalDbContextType, services)
        {
            EntityOptions = new Dictionary<Type, object>();
        }

        public void Entity<TEntity>([NotNull] Action<SkywalkerEntityOptions<TEntity>> optionsAction) where TEntity : IEntity
        {
            Services.Configure<SkywalkerEntityOptions>(options =>
            {
                options.Entity(optionsAction);
            });
        }
    }
}