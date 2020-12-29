using Skywalker;
using Skywalker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public class SkywalkerEntityOptions<TEntity> where TEntity : IEntity
    {
        public static SkywalkerEntityOptions<TEntity> Empty { get; } = new SkywalkerEntityOptions<TEntity>();

        public Func<IQueryable<TEntity>, IQueryable<TEntity>>? DefaultWithDetailsFunc { get; set; }
    }

    public class SkywalkerEntityOptions
    {
        private readonly IDictionary<Type, object> _options;

        public SkywalkerEntityOptions()
        {
            _options = new Dictionary<Type, object>();
        }

        public SkywalkerEntityOptions<TEntity>? GetOrNull<TEntity>() where TEntity : IEntity
        {
            return _options.GetOrDefault(typeof(TEntity)) as SkywalkerEntityOptions<TEntity>;
        }

        public void Entity<TEntity>([NotNull] Action<SkywalkerEntityOptions<TEntity>> optionsAction) where TEntity : IEntity
        {
            Check.NotNull(optionsAction, nameof(optionsAction));
            SkywalkerEntityOptions<TEntity>? options = _options.GetOrAdd(typeof(TEntity), () => new SkywalkerEntityOptions<TEntity>()) as SkywalkerEntityOptions<TEntity>;
            optionsAction.Invoke(options!);
        }
    }
}
