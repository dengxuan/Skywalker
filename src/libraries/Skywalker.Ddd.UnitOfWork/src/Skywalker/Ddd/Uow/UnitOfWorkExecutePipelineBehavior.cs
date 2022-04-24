using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Ddd.Application.Dtos.Abstractions;
using Skywalker.Ddd.Application.Pipeline;
using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow;

public class UnitOfWorkExecutePipelineBehavior: IPipelineBehavior
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly UnitOfWorkDefaultOptions _defaultOptions;
    private readonly ILogger<UnitOfWorkExecutePipelineBehavior> _logger;

    public UnitOfWorkExecutePipelineBehavior(IUnitOfWorkManager unitOfWorkManager, IOptions<UnitOfWorkDefaultOptions> options, ILogger<UnitOfWorkExecutePipelineBehavior> logger)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _defaultOptions = options.Value;
        _logger = logger;
    }

    public async ValueTask<TResponse?> HandleAsync<TRequest, TResponse>(TRequest message, ApplicationHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken) where TRequest : notnull, IRequestDto where TResponse : notnull, IResponseDto
    {
        if (!UnitOfWorkHelper.IsUnitOfWorkMethod(next.Method, out var unitOfWorkAttribute))
        {
            return await next(message, cancellationToken);
        }

        using var uow = _unitOfWorkManager.Begin(CreateOptions(next.Method.Name, unitOfWorkAttribute));
        _logger.LogDebug("Begin Unit of work:[{uow.Id}]", uow.Id);
        var output = await next(message, cancellationToken);
        await uow.CompleteAsync(cancellationToken);
        _logger.LogDebug("Complete Unit of work:[{uow.Id}]", uow.Id);
        return output;
    }

    public ValueTask HandleAsync<TRequest>(TRequest request, ApplicationHandlerDelegate<TRequest> next, CancellationToken cancellationToken) where TRequest : notnull, IRequestDto => throw new NotImplementedException();


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
