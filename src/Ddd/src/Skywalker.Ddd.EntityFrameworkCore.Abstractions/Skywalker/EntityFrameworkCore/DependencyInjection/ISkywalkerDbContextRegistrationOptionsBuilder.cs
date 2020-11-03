using System;
using System.Diagnostics.CodeAnalysis;
using Skywalker.DependencyInjection;
using Skywalker.Domain.Entities;

namespace Skywalker.EntityFrameworkCore.DependencyInjection
{
    public interface ISkywalkerDbContextRegistrationOptionsBuilder : ISkywalkerCommonDbContextRegistrationOptionsBuilder
    {
        void Entity<TEntity>([NotNull] Action<SkywalkerEntityOptions<TEntity>> optionsAction)
            where TEntity : IEntity;
    }
}