using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Infrastructure.Abstractions
{
    public interface IDbContext
    {
        IDataCollection<T> DataCollection<T>() where T : class;

        int SaveChanges();

        int SaveChanges(bool acceptAllChangesOnSuccess);

        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        void AddRange([NotNull] IEnumerable<object> entities);

        void AddRange([NotNull] params object[] entities);

        Task AddRangeAsync([NotNull] params object[] entities);

        Task AddRangeAsync([NotNull] IEnumerable<object> entities, CancellationToken cancellationToken = default);

        void AttachRange([NotNull] IEnumerable<object> entities);

        void AttachRange([NotNull] params object[] entities);

        object Find([NotNull] Type entityType, [NotNull] params object[] keyValues);

        TEntity Find<TEntity>([NotNull] params object[] keyValues) where TEntity : class;

        ValueTask<object> FindAsync([NotNull] Type entityType, [NotNull] object[] keyValues, CancellationToken cancellationToken);

        ValueTask<TEntity> FindAsync<TEntity>([NotNull] object[] keyValues, CancellationToken cancellationToken) where TEntity : class;

        ValueTask<TEntity> FindAsync<TEntity>([NotNull] params object[] keyValues) where TEntity : class;

        ValueTask<object> FindAsync([NotNull] Type entityType, [NotNull] params object[] keyValues);

        void RemoveRange([NotNull] IEnumerable<object> entities);

        void RemoveRange([NotNull] params object[] entities);

        void UpdateRange([NotNull] params object[] entities);

        void UpdateRange([NotNull] IEnumerable<object> entities);
    }
}
