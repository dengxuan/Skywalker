
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.AspNetCore.Uow;

public class UnitOfWorkMiddleware(
    RequestDelegate next,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<UnitOfWorkMiddleware> logger)
{

    private static UnitOfWorkOptions CreateOptions(IServiceProvider serviceProvider, UnitOfWorkAttribute? unitOfWorkAttribute)
    {
        var options = new UnitOfWorkOptions();

        unitOfWorkAttribute?.SetOptions(options);

        if ((unitOfWorkAttribute?.IsTransactional) != null)
        {
            return options;
        }

        var defaultOptions = serviceProvider.GetRequiredService<IOptions<UnitOfWorkDefaultOptions>>().Value;
        var transactional = serviceProvider.GetRequiredService<IUnitOfWorkTransactionBehaviourProvider>().IsTransactional;
        if (transactional != null)
        {
            options.IsTransactional = defaultOptions.CalculateIsTransactional(transactional.Value);
            return options;
        }
        var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
        var httpMethod = httpContextAccessor?.HttpContext?.Request?.Method;

        switch (httpMethod)
        {
            case "GET":
            case "HEAD":
            case "OPTIONS":
                options.IsTransactional = defaultOptions.CalculateIsTransactional(false);
                return options;
            case "POST":
            case "PUT":
            case "PATCH":
            case "DELETE":
                options.IsTransactional = defaultOptions.CalculateIsTransactional(true);
                return options;
        }

        return options;
    }

    public async Task InvokeAsync(HttpContext context, IUnitOfWorkManager unitOfWorkManager)
    {
        logger.LogDebug("A request is intercepted and a unit of work is started.");
        // Skip UOW for non-action requests
        var endPoint = context.GetEndpoint();
        if (endPoint == null)
        {
            logger.LogDebug("The request is not an action request. Proceeding without a unit of work.");
            await next(context);
            return;
        }
        // Skip UOW if there is no action descriptor
        var actionDescriptor = endPoint.Metadata.GetMetadata<ActionDescriptor>();
        if (actionDescriptor == null)
        {
            logger.LogDebug("The request is not an action request. Proceeding without a unit of work.");
            await next(context);
            return;
        }

        //Check if this action has a UnitOfWorkAttribute and if it is disabled
        var methodInfo = actionDescriptor.GetMethodInfo();
        if (!UnitOfWorkHelper.IsUnitOfWorkMethod(methodInfo, out var unitOfWorkAttribute) || unitOfWorkAttribute?.IsDisabled == true)
        {
            logger.LogDebug("The action is not a unit of work action. Proceeding without a unit of work.");
            await next(context);
            return;
        }

        logger.LogDebug("The action is a unit of work action. Trying to start a unit of work.");
        using var scope = serviceScopeFactory.CreateScope();
        var options = CreateOptions(scope.ServiceProvider, unitOfWorkAttribute);

        //Trying to begin a reserved UOW by UnitOfWorkMiddleware
        if (unitOfWorkManager.TryBeginReserved(UnitOfWork.UnitOfWorkReservationName, options))
        {
            logger.LogDebug("A reserved unit of work is started.");
            await next(context);

            //if (unitOfWorkManager.Current != null)
            //{
            //    await unitOfWorkManager.Current.SaveChangesAsync();
            //}
            logger.LogDebug("The reserved unit of work is completed.");
            return;
        }

        //Begin a new UOW
        logger.LogDebug("No reserved unit of work is found. Starting a new unit of work.");
        using var uow = unitOfWorkManager.Begin(options);
        try
        {
            await next(context);
            await uow.CompleteAsync();
            logger.LogDebug("The unit of work is completed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while executing the unit of work. Rolling back the transaction.");
            await uow.RollbackAsync();
            throw;
        }
    }
}
