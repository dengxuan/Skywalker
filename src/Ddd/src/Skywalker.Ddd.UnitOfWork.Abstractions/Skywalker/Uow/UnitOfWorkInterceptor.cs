using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Aspects.DynamicProxy;
using Skywalker.Aspects.Interceptors;
using Skywalker.DependencyInjection;
using Skywalker.Uow.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Skywalker.Uow
{
    public class UnitOfWorkInterceptor
    {
        private readonly InterceptDelegate _next;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly AbpUnitOfWorkDefaultOptions _defaultOptions;
        private readonly ILogger<UnitOfWorkInterceptor> _logger;

        public UnitOfWorkInterceptor(InterceptDelegate next, IUnitOfWorkManager unitOfWorkManager, IOptions<AbpUnitOfWorkDefaultOptions> options, ILogger<UnitOfWorkInterceptor> logger)
        {
            _next = next;
            _unitOfWorkManager = unitOfWorkManager;
            _defaultOptions = options.Value;
            _logger = logger;
        }

        public async Task InvokeAsync(InvocationContext context)
        {
            if (!UnitOfWorkHelper.IsUnitOfWorkMethod(context.Invocation.Method, out var unitOfWorkAttribute))
            {
                await _next(context);
                return;
            }

            using var uow = _unitOfWorkManager.Begin(CreateOptions(context.Invocation, unitOfWorkAttribute));
            _logger.LogDebug($"Begin Unit of work:[{uow.Id}]");
            await _next(context);
            await uow.CompleteAsync();
            _logger.LogDebug($"Complete Unit of work:[{uow.Id}]");
        }

        private AbpUnitOfWorkOptions CreateOptions(IInvocation invocation, [MaybeNull] UnitOfWorkAttribute unitOfWorkAttribute)
        {
            var options = new AbpUnitOfWorkOptions();

            unitOfWorkAttribute?.SetOptions(options);

            if (unitOfWorkAttribute?.IsTransactional == null)
            {
                options.IsTransactional = _defaultOptions.CalculateIsTransactional(
                    autoValue: !invocation.Method.Name.StartsWith("Find", StringComparison.InvariantCultureIgnoreCase)
                );
            }

            return options;
        }
    }
}
