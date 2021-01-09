using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Aspects.DynamicProxy;
using Skywalker.Aspects.Interceptors;
using Skywalker.Ddd.UnitOfWork.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.UnitOfWork
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
            if (!UnitOfWorkHelper.IsUnitOfWorkMethod(context.Method, out var unitOfWorkAttribute))
            {
                await _next(context);
                return;
            }
            using var uow = _unitOfWorkManager.Begin(CreateOptions(context.TargetMethod, unitOfWorkAttribute!));
            try
            {
                _logger.LogInformation("Begin Unit of work:[{0}]", uow.Id);
                await _next(context);
                await uow.CompleteAsync();
                _logger.LogInformation("Complete Unit of work:[{0}]", uow.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Unit of work:[{uow.Id}] Exception:", uow.Id, ex.Message);
            }
        }

        private AbpUnitOfWorkOptions CreateOptions(MethodInfo method, [MaybeNull] UnitOfWorkAttribute unitOfWorkAttribute)
        {
            var options = new AbpUnitOfWorkOptions();

            unitOfWorkAttribute?.SetOptions(options);

            if (unitOfWorkAttribute?.IsTransactional == null)
            {
                options.IsTransactional = _defaultOptions.CalculateIsTransactional(
                    autoValue: !method.Name.StartsWith("Get", StringComparison.InvariantCultureIgnoreCase)
                );
            }

            return options;
        }
    }
}
