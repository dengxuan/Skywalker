using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Ddd.Application.Dtos.Abstractions;
using Skywalker.Ddd.Application.Wrappers;

namespace Skywalker.Ddd.Application;

internal class DefaultApplication : IApplication
{
    private static readonly ConcurrentDictionary<Type, RequestHandlerWrapper> s_requestHandlers = new();
    private static readonly ConcurrentDictionary<Type, ResponseHandlerWrapper> s_responseHandlers = new();

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

        var handler = (ResponseHandlerWrapper<TResponse>)s_responseHandlers.GetOrAdd(requestType, t =>
        {
            var genericType = typeof(ResponseHandlerWrapper<,>).MakeGenericType(t, typeof(TResponse));
            return (ResponseHandlerWrapper)(ActivatorUtilities.CreateInstance(_serviceProvider, genericType) ?? throw new InvalidOperationException($"Could not create wrapper type for {t}"));
        });
        return handler.Handle(request, cancellationToken);
    }

    public ValueTask ExecuteAsync(IRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();
        var handler = s_requestHandlers.GetOrAdd(requestType, requestTypeKey =>
        {
            var genericType = typeof(RequestHandlerWrapper<>).MakeGenericType(requestTypeKey);
            return (RequestHandlerWrapper)(ActivatorUtilities.CreateInstance(_serviceProvider, genericType) ?? throw new InvalidOperationException($"Could not create wrapper type for {requestTypeKey}"));
        });
        var w = (RequestHandlerWrapper)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(RequestHandlerWrapper<>).MakeGenericType(requestType));
        return handler.Handle(request, cancellationToken);
    }
}
