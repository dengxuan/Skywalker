using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.Ddd.Infrastructure;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Ddd.Infrastructure.DbContextConfiguration;
using Skywalker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Domain.Repositories.EntityFrameworkCore
{
    public class DbContextRepository<TDbContext, TEntity> : RepositoryBase<TEntity>, IDbContextRepository<TEntity> where TDbContext : IDbContext where TEntity : class, IEntity
    {
        IDbContext IDbContextRepository<TEntity>.DbContext => DbContext.As<IDbContext>();

        public virtual IDataCollection<TEntity> DataCollection => DbContext.DataCollection<TEntity>();

        protected virtual TDbContext DbContext => _dbContextProvider.GetDbContext();

        protected virtual SkywalkerEntityOptions<TEntity> SkywalkerEntityOptions => _entityOptionsLazy.Value;

        private readonly IDbContextProvider<TDbContext> _dbContextProvider;
        private readonly Lazy<SkywalkerEntityOptions<TEntity>> _entityOptionsLazy;

        public DbContextRepository(IDbContextProvider<TDbContext> dbContextProvider, IServiceProvider serviceProvider)
        {
            _dbContextProvider = dbContextProvider;
            ServiceProvider = serviceProvider;
            _entityOptionsLazy = new Lazy<SkywalkerEntityOptions<TEntity>>(
                () => ServiceProvider
                          .GetRequiredService<IOptions<SkywalkerEntityOptions>>()
                          .Value
                          .GetOrNull<TEntity>() ?? SkywalkerEntityOptions<TEntity>.Empty
            );
        }

        protected override IQueryable<TEntity> GetQueryable()
        {
            throw new NotImplementedException();
        }

        public override Task<TEntity> FindAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<TEntity> InsertAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<TEntity> UpdateAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task DeleteAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class DbContextRepository<TDbContext, TEntity, TKey> : DbContextRepository<TDbContext, TEntity>,
        IDbContextRepository<TEntity, TKey>,
        ISupportsExplicitLoading<TEntity, TKey>

        where TDbContext : IDbContext
        where TEntity : class, IEntity<TKey>
    {
        public DbContextRepository(IDbContextProvider<TDbContext> dbContextProvider, IServiceProvider serviceProvider)
            : base(dbContextProvider, serviceProvider)
        {

        }

        public Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task EnsureCollectionLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
        {
            throw new NotImplementedException();
        }

        public Task EnsurePropertyLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyExpression, CancellationToken cancellationToken) where TProperty : class
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FindAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
