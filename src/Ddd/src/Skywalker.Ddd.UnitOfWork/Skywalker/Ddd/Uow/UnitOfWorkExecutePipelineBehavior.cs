using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Application;
using Skywalker.Application.Abstractions;
using Skywalker.Application.Dtos.Contracts;
using Skywalker.Ddd.Uow.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.Uow
{
    public class UnitOfWorkExecutePipelineBehavior<TOutputDto> : IExecutePipelineBehavior<TOutputDto> where TOutputDto : IEntityDto
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly AbpUnitOfWorkDefaultOptions _defaultOptions;
        private readonly ILogger<UnitOfWorkExecutePipelineBehavior<TOutputDto>> _logger;

        public UnitOfWorkExecutePipelineBehavior(IUnitOfWorkManager unitOfWorkManager, IOptions<AbpUnitOfWorkDefaultOptions> options, ILogger<UnitOfWorkExecutePipelineBehavior<TOutputDto>> logger)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _defaultOptions = options.Value;
            _logger = logger;
        }

        public async Task<TOutputDto?> HandleAsync(ExecuteHandlerDelegate<TOutputDto> next, CancellationToken cancellationToken = default)
        {
            if (!UnitOfWorkHelper.IsUnitOfWorkMethod(next.Method, out var unitOfWorkAttribute))
            {
                return await next(cancellationToken);
            }

            using var uow = _unitOfWorkManager.Begin(CreateOptions(next.Method.Name, unitOfWorkAttribute));
            _logger.LogDebug($"Begin Unit of work:[{uow.Id}]");
            var output = await next(cancellationToken);
            await uow.CompleteAsync(cancellationToken);
            _logger.LogDebug($"Complete Unit of work:[{uow.Id}]");
            return output;
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
