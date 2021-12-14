using Skywalker.DependencyInjection;
using Skywalker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    public class SkywalkerDbContextRegistrationOptions : SkywalkerCommonDbContextRegistrationOptions, ISkywalkerDbContextRegistrationOptionsBuilder
    {
        public Dictionary<Type, object> EntityOptions { get; }

        public SkywalkerDbContextRegistrationOptions(Type dbContextType, IServiceCollection services) : base(dbContextType, services)
        {
            EntityOptions = new Dictionary<Type, object>();
        }

        public void Entity<TEntity>(Action<SkywalkerEntityOptions<TEntity>> optionsAction) where TEntity : IEntity
        {
            Services.Configure<SkywalkerEntityOptions>(options =>
            {
                options.Entity(optionsAction);
            });
        }
    }
}