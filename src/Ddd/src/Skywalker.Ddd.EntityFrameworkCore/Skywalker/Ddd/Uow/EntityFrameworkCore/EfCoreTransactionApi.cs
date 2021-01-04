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
        public IDbContextTransaction DbContextTransaction { get; }
        public ISkywalkerDbContext StarterDbContext { get; }
        public List<ISkywalkerDbContext> AttendedDbContexts { get; }

        public EfCoreTransactionApi(IDbContextTransaction dbContextTransaction, ISkywalkerDbContext starterDbContext)
        {
            DbContextTransaction = dbContextTransaction;
            StarterDbContext = starterDbContext;
            AttendedDbContexts = new List<ISkywalkerDbContext>();
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

        public void Dispose()
        {
            DbContextTransaction.Dispose();
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
    }
}