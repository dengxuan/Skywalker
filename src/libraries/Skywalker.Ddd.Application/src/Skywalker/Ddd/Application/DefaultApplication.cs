using System.Collections.Concurrent;
using MediatR.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application;

internal class DefaultApplication : IApplication
{
    private static readonly ConcurrentDictionary<Type, RequestHandlerWrapper> _requestHandlers = new();
    private static readonly ConcurrentDictionary<Type, ResponseHandlerWrapper> _responseHandlers = new();
    private readonly IServiceProvider _serviceProvider;

    public DefaultApplication(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ValueTask<TResponse> ExecuteAsync<TResponse>(IRequestDto<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();

        var handler = (ResponseHandlerWrapper<TResponse>)_responseHandlers.GetOrAdd(requestType,
            t => (ResponseHandlerWrapper)(ActivatorUtilities.CreateInstance(_serviceProvider, typeof(ResponseHandlerWrapper<,>).MakeGenericType(t, typeof(TResponse)))
                                             ?? throw new InvalidOperationException($"Could not create wrapper type for {t}")));

        return handler.Handle(request, cancellationToken);
    }

    public ValueTask ExecuteAsync(IRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();

        var handler = _requestHandlers.GetOrAdd(requestType,
            requestTypeKey =>
            {
                return (RequestHandlerWrapper)(ActivatorUtilities.CreateInstance(_serviceProvider, typeof(RequestHandlerWrapper<>).MakeGenericType(requestTypeKey))
                                                             ?? throw new InvalidOperationException($"Could not create wrapper type for {requestTypeKey}"));
            });

        return handler.Handle(request, cancellationToken);
    }
}
