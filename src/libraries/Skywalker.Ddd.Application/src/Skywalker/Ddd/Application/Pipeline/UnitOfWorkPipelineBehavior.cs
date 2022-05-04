using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Ddd.Application.Dtos.Abstractions;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Application.Pipeline;

public class UnitOfWorkPipelineBehavior : IPipelineBehavior
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly UnitOfWorkDefaultOptions _defaultOptions;
    private readonly ILogger<UnitOfWorkPipelineBehavior> _logger;

    private readonly InterceptDelegate _next;

    public UnitOfWorkPipelineBehavior(InterceptDelegate next, IUnitOfWorkManager unitOfWorkManager, IOptions<UnitOfWorkDefaultOptions> options, ILogger<UnitOfWorkPipelineBehavior> logger)
    {
        _next = next;
        _unitOfWorkManager = unitOfWorkManager;
        _defaultOptions = options.Value;
        _logger = logger;
    }


    public async ValueTask InvokeAsync(PipelineContext context)
    {
        if (!UnitOfWorkHelper.IsUnitOfWorkMethod(context.Handler.GetType(), out var unitOfWorkAttribute))
        {
            await _next(context);
            return;
        }
        _logger.LogInformation("Begin Invoke UnitOfWorkPipelineBehavior.InvokeAsync");
        using var uow = _unitOfWorkManager.Begin(CreateOptions(context.Handler.GetType().Name, unitOfWorkAttribute));
        _logger.LogDebug("Begin Unit of work:[{uow.Id}]", uow.Id);
        await _next(context);
        await uow.CompleteAsync();
        _logger.LogDebug("Complete Unit of work:[{uow.Id}]", uow.Id);
    }

    //public async ValueTask<TResponse?> HandleAsync<TRequest, TResponse>(TRequest request, ApplicationHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken) where TRequest : notnull, IRequestDto where TResponse : notnull, IResponseDto
    //{
    //    if (!UnitOfWorkHelper.IsUnitOfWorkMethod(next.Method, out var unitOfWorkAttribute))
    //    {
    //        return await next(request, cancellationToken);
    //    }

    //    using var uow = _unitOfWorkManager.Begin(CreateOptions(next.Method.Name, unitOfWorkAttribute));
    //    _logger.LogDebug("Begin Unit of work:[{uow.Id}]", uow.Id);
    //    var output = await next(request, cancellationToken);
    //    await uow.CompleteAsync(cancellationToken);
    //    _logger.LogDebug("Complete Unit of work:[{uow.Id}]", uow.Id);
    //    return output;
    //}

    //public async ValueTask HandleAsync<TRequest>(TRequest request, ApplicationHandlerDelegate<TRequest> next, CancellationToken cancellationToken) where TRequest : notnull, IRequestDto
    //{
    //    if (!UnitOfWorkHelper.IsUnitOfWorkMethod(next.Method, out var unitOfWorkAttribute))
    //    {
    //        await next(request, cancellationToken);
    //        return;
    //    }
    //    using var uow = _unitOfWorkManager.Begin(CreateOptions(next.Method.Name, unitOfWorkAttribute));
    //    _logger.LogDebug("Begin Unit of work:[{uow.Id}]", uow.Id);
    //    await next(request, cancellationToken);
    //    await uow.CompleteAsync(cancellationToken);
    //    _logger.LogDebug("Complete Unit of work:[{uow.Id}]", uow.Id);
    //}

    private UnitOfWorkOptions CreateOptions(string methodName, UnitOfWorkAttribute? unitOfWorkAttribute)
    {
        var options = new UnitOfWorkOptions();

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
