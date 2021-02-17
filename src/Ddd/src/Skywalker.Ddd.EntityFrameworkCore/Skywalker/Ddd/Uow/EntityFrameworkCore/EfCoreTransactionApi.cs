using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.UnitOfWork.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.UnitOfWork.EntityFrameworkCore
{
    public class EfCoreTransactionApi : ITransactionApi, ISupportsRollback
    {
        private bool _disposedValue;

        public IDbContextTransaction DbContextTransaction { get; }
        public DbContext StarterDbContext { get; }
        public List<DbContext> AttendedDbContexts { get; }

        public EfCoreTransactionApi(IDbContextTransaction dbContextTransaction, DbContext starterDbContext)
        {
            DbContextTransaction = dbContextTransaction;
            StarterDbContext = starterDbContext;
            AttendedDbContexts = new List<DbContext>();
        }

        public Task CommitAsync()
        {
            Commit();
            return Task.CompletedTask;
        }
        
        protected void Commit()
        {
            DbContextTransaction.Commit();

            foreach (var dbContext in AttendedDbContexts)
            {
                if (dbContext.As<DbContext>().HasRelationalTransactionManager())
                {
                    continue; //Relational databases use the shared transaction
                }

                dbContext.Database.CommitTransaction();
            }
        }

        public void Rollback()
        {
            DbContextTransaction.Rollback();
        }

        public Task RollbackAsync(CancellationToken cancellationToken)
        {
            DbContextTransaction.Rollback();
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    DbContextTransaction.Dispose();
                    foreach (var dbContext in AttendedDbContexts)
                    {
                        dbContext.Dispose();
                    }
                    StarterDbContext.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        /// <summary>
        /// override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        /// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        /// </summary>
        ~EfCoreTransactionApi()
        {
            Dispose(disposing: false);
        }

        /// <summary>
        /// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}