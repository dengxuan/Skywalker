using Skywalker.DependencyInjection;
using Skywalker.Domain.Entities;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface ISkywalkerDbContextRegistrationOptionsBuilder : ISkywalkerCommonDbContextRegistrationOptionsBuilder
    {
        void Entity<TEntity>(Action<SkywalkerEntityOptions<TEntity>> optionsAction) where TEntity : IEntity;
    }
}