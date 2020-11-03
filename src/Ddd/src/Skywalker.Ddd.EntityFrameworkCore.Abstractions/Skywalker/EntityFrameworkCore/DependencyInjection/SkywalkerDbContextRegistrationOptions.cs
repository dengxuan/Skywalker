using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.DependencyInjection;
using Skywalker.Domain.Entities;

namespace Skywalker.EntityFrameworkCore.DependencyInjection
{
    public class SkywalkerDbContextRegistrationOptions : SkywalkerCommonDbContextRegistrationOptions, ISkywalkerDbContextRegistrationOptionsBuilder
    {
        public Dictionary<Type, object> AbpEntityOptions { get; }

        public SkywalkerDbContextRegistrationOptions(Type originalDbContextType, IServiceCollection services)
            : base(originalDbContextType, services)
        {
            AbpEntityOptions = new Dictionary<Type, object>();
        }

        public void Entity<TEntity>(Action<SkywalkerEntityOptions<TEntity>> optionsAction) where TEntity : IEntity
        {
            Services.Configure<AbpEntityOptions>(options =>
            {
                options.Entity(optionsAction);
            });
        }
    }
}