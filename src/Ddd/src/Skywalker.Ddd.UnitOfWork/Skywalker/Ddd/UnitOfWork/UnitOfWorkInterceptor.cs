using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Aspects.Interceptors;
using Skywalker.Ddd.UnitOfWork.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

namespace Skywalker.Ddd.UnitOfWork
{
    public class UnitOfWorkInterceptor
    {
        private readonly UnitOfWorkDefaultOptions _defaultOptions;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ILogger<UnitOfWorkInterceptor> _logger;

        public UnitOfWorkInterceptor(IOptions<UnitOfWorkDefaultOptions> options, IUnitOfWorkManager unitOfWorkManager, ILogger<UnitOfWorkInterceptor> logger)
        {
            _logger = logger;
            _defaultOptions = options.Value;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task InvokeAsync(InvocationContext context)
        {
            if (!UnitOfWorkHelper.IsUnitOfWorkMethod(context.Invocation.Method, out var unitOfWorkAttribute))
            {
                await context.ProceedAsync();
                return;
            }
            using var uow = _unitOfWorkManager.Begin(CreateOptions(context.Invocation.Method, unitOfWorkAttribute!));
            try
            {
                _logger.LogInformation("开始事务:[{0}]", uow.Id);
                await context.ProceedAsync();
                await uow.CompleteAsync();
                _logger.LogInformation("提交事务:[{0}]", uow.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "事务异常:[{0}]", uow.Id);
                await uow.RollbackAsync();
                ex.ReThrow();
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
