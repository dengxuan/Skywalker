using Microsoft.Extensions.Logging;
using Skywalker.Ddd.Queries.Abstractions;
using Skywalker.Ddd.UnitOfWork.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.UnitOfWork
{
    public class UniyOfWorkSearchingPipeline<TQuery, TOutout> : ISearchingPipelineBehavior<TQuery, TOutout> where TQuery : IQuery<TOutout>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ILogger<UniyOfWorkSearchingPipeline<TQuery, TOutout>> _logger;

        public UniyOfWorkSearchingPipeline(IUnitOfWorkManager unitOfWorkManager, ILogger<UniyOfWorkSearchingPipeline<TQuery, TOutout>> logger)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _logger = logger;
        }

        public async Task<TQuery> HandleAsync(QueryHandlerDelegate<TQuery> next, CancellationToken cancellationToken = default)
        {
            using var uow = _unitOfWorkManager.Begin();
            TQuery outout = default;
            try
            {
                _logger.LogInformation("开始事务:[{0}]", uow.Id);
                outout = await next();
                await uow.CompleteAsync();
                _logger.LogInformation("提交事务:[{0}]", uow.Id);
                return outout;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"事务异常:[{uow.Id}] 异常信息:", uow.Id, ex.Message);
                await uow.RollbackAsync();
                ex.ReThrow();
            }
            return outout;
        }

        public async Task<TOutout> HandleAsync(TQuery querier, QueryHandlerDelegate<TOutout> next, CancellationToken cancellationToken = default)
        {
            using var uow = _unitOfWorkManager.Begin();
            try
            {
                _logger.LogInformation("开始事务:[{0}]", uow.Id);
                TOutout output = await next();
                await uow.CompleteAsync();
                _logger.LogInformation("提交事务:[{0}]", uow.Id);
                return output;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"事务异常:[{uow.Id}] 异常信息:", uow.Id, ex.Message);
                await uow.RollbackAsync();
                ex.ReThrow();
            }
            return default;
        }
    }
}
