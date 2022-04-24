using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application;

internal class DefaultApplication : IApplication
{
    private readonly IApplicationHandlerProvider _handlerProvider;

    public DefaultApplication(IApplicationHandlerProvider handlerProvider)
    {
        _handlerProvider = handlerProvider;
    }

    public ValueTask ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequestDto
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        return _handlerProvider.HandleAsync(request, cancellationToken);
    }

    public ValueTask<TResponse?> ExecuteAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequestDto where TResponse : IResponseDto
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        return _handlerProvider.HandleAsync<TRequest, TResponse>(request, cancellationToken);
    }
}
