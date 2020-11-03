using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using Skywalker.Domain.Entities;

namespace Skywalker.EntityFrameworkCore.DependencyInjection
{
    public class SkywalkerEntityOptions<TEntity>
        where TEntity : IEntity
    {
        public static SkywalkerEntityOptions<TEntity> Empty { get; } = new SkywalkerEntityOptions<TEntity>();

        public Func<IQueryable<TEntity>, IQueryable<TEntity>> DefaultWithDetailsFunc { get; set; }
    }

    public class AbpEntityOptions
    {
        private readonly IDictionary<Type, object> _options;

        public AbpEntityOptions()
        {
            _options = new Dictionary<Type, object>();
        }

        public SkywalkerEntityOptions<TEntity> GetOrNull<TEntity>()
            where TEntity : IEntity
        {
            return _options.GetOrDefault(typeof(TEntity)) as SkywalkerEntityOptions<TEntity>;
        }

        public void Entity<TEntity>([NotNull] Action<SkywalkerEntityOptions<TEntity>> optionsAction)
            where TEntity : IEntity
        {
            Check.NotNull(optionsAction, nameof(optionsAction));

            optionsAction(
                _options.GetOrAdd(
                    typeof(TEntity),
                    () => new SkywalkerEntityOptions<TEntity>()
                ) as SkywalkerEntityOptions<TEntity>
            );
        }
    }
}