using Skywalker.DependencyInjection;
using Skywalker.Domain.Entities;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.Infrastructure.DbContextConfiguration
{
    public interface ISkywalkerDbContextRegistrationOptionsBuilder : ISkywalkerCommonDbContextRegistrationOptionsBuilder
    {
        void Entity<TEntity>([NotNull] Action<SkywalkerEntityOptions<TEntity>> optionsAction) where TEntity : IEntity;
    }
}