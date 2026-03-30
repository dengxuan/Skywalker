// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.Extensions.DynamicProxies;

namespace Skywalker.Ddd.Uow;

/// <summary>
/// Unit of Work 拦截器，自动管理工作单元的生命周期。
/// </summary>
/// <remarks>
/// 使用示例：
/// <code>
/// // IOrderService 继承 IInterceptable 将自动生成代理
/// [UnitOfWork]
/// public class OrderService : IOrderService, IScopedDependency
/// {
///     public async Task CreateOrderAsync(OrderDto dto) { ... }
/// }
/// </code>
/// </remarks>
public class UnitOfWorkInterceptor(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<UnitOfWorkInterceptor> logger) : IInterceptor, ITransientDependency
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly ILogger<UnitOfWorkInterceptor> _logger = logger;

    private static UnitOfWorkOptions CreateOptions(IServiceProvider serviceProvider, IMethodInvocation invocation, UnitOfWorkAttribute? unitOfWorkAttribute)
    {
        var options = new UnitOfWorkOptions();

        unitOfWorkAttribute?.SetOptions(options);

        if (unitOfWorkAttribute?.IsTransactional == null)
        {
            var defaultOptions = serviceProvider.GetRequiredService<IOptions<UnitOfWorkDefaultOptions>>().Value;
            var transactional = serviceProvider.GetRequiredService<IUnitOfWorkTransactionBehaviourProvider>().IsTransactional;
            if (transactional != null)
            {
                options.IsTransactional = defaultOptions.CalculateIsTransactional(transactional.Value);
                return options;
            }
            var methodName = invocation.Method.Name;
            if(methodName.StartsWith("Get", StringComparison.InvariantCultureIgnoreCase))
            {
                options.IsTransactional = defaultOptions.CalculateIsTransactional(false);
                return options;
            }

            if (methodName.StartsWith("Find", StringComparison.InvariantCultureIgnoreCase))
            {
                options.IsTransactional = defaultOptions.CalculateIsTransactional(false);
                return options;
            }
        }

        return options;
    }

    /// <inheritdoc />
    public async Task InterceptAsync(IMethodInvocation invocation)
    {
        _logger.LogDebug("A method is intercepted and a unit of work is started.");
        if (!UnitOfWorkHelper.IsUnitOfWorkMethod(invocation.Method, out var unitOfWorkAttribute))
        {
            _logger.LogDebug("The method is not a unit of work method. Proceeding without a unit of work.");
            await invocation.ProceedAsync();
            return;
        }

        _logger.LogDebug("The method is a unit of work method. Trying to start a unit of work.");
        using var scope = _serviceScopeFactory.CreateScope();
        var options = CreateOptions(scope.ServiceProvider, invocation, unitOfWorkAttribute);

        var unitOfWorkManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

        //Trying to begin a reserved UOW by UnitOfWorkInterceptor
        if (unitOfWorkManager.TryBeginReserved(UnitOfWork.UnitOfWorkReservationName, options))
        {
            _logger.LogDebug("A reserved unit of work is started.");
            await invocation.ProceedAsync();
            _logger.LogDebug("The reserved unit of work is completed.");

            return;
        }

        _logger.LogDebug("No reserved unit of work is found. Starting a new unit of work.");
        using var uow = unitOfWorkManager.Begin(options);
        try
        {
            await invocation.ProceedAsync();
            await uow.CompleteAsync();
            _logger.LogDebug("The unit of work is completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while executing the unit of work. Rolling back the transaction.");
            await uow.RollbackAsync();
            throw;
        }
    }
}
