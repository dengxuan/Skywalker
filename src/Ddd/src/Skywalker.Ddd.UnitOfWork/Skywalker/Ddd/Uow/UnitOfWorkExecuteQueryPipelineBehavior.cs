using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Application.Abstractions;
using Skywalker.Application.Dtos.Contracts;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Uow
{
    public class UnitOfWorkExecuteQueryPipelineBehavior<TInputDto> : IExecuteNonQueryPipelineBehavior<TInputDto> where TInputDto : IEntityDto
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly AbpUnitOfWorkDefaultOptions _defaultOptions;
        private readonly ILogger<UnitOfWorkExecuteQueryPipelineBehavior<TInputDto>> _logger;

        public UnitOfWorkExecuteQueryPipelineBehavior(IUnitOfWorkManager unitOfWorkManager, IOptions<AbpUnitOfWorkDefaultOptions> options, ILogger<UnitOfWorkExecuteQueryPipelineBehavior<TInputDto>> logger)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _defaultOptions = options.Value;
            _logger = logger;
        }

        public async Task HandleAsync(TInputDto inputDto, ExecuteNonQueryHandlerDelegate<TInputDto> next, CancellationToken cancellationToken = default)
        {
            if (!UnitOfWorkHelper.IsUnitOfWorkMethod(next.Method, out var unitOfWorkAttribute))
            {
                await next(inputDto, cancellationToken);
                return;
            }

            using var uow = _unitOfWorkManager.Begin(CreateOptions(next.Method.Name, unitOfWorkAttribute));
            _logger.LogDebug($"Begin Unit of work:[{uow.Id}]");
            await next(inputDto, cancellationToken);
            await uow.CompleteAsync(cancellationToken);
            _logger.LogDebug($"Complete Unit of work:[{uow.Id}]");
        }

        private AbpUnitOfWorkOptions CreateOptions(string methodName, [MaybeNull] UnitOfWorkAttribute? unitOfWorkAttribute)
        {
            var options = new AbpUnitOfWorkOptions();

            unitOfWorkAttribute?.SetOptions(options);

            if (unitOfWorkAttribute?.IsTransactional == null)
            {
                options.IsTransactional = _defaultOptions.CalculateIsTransactional(
                    autoValue: !methodName.StartsWith("Find", StringComparison.InvariantCultureIgnoreCase)
                );
            }

            return options;
        }
    }
}
