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

    //public Task<object?> ExecuteAsync(object request, CancellationToken cancellationToken = default)
    //{
    //    if (request == null)
    //    {
    //        throw new ArgumentNullException(nameof(request));
    //    }
    //    var requestType = request.GetType();
    //    var handler = _requestHandlers.GetOrAdd(requestType,
    //        static requestTypeKey =>
    //        {
    //            var requestInterfaceType = requestTypeKey
    //                .GetInterfaces()
    //                .FirstOrDefault(static i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestDto<>));

    //            if (requestInterfaceType is null)
    //            {
    //                throw new ArgumentException($"{requestTypeKey.Name} does not implement {nameof(IRequestDto)}", nameof(request));
    //            }

    //            var responseType = requestInterfaceType.GetGenericArguments()[0];
    //            var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestTypeKey, responseType);

    //            return (RequestHandlerBase)(Activator.CreateInstance(wrapperType)
    //                                        ?? throw new InvalidOperationException($"Could not create wrapper for type {wrapperType}"));
    //        });

    //    // call via dynamic dispatch to avoid calling through reflection for performance reasons
    //    return handler.Handle(request, cancellationToken, _serviceFactory);
    //}

    //public async ValueTask ExecuteAsync(IRequestDto request, CancellationToken cancellationToken = default)
    //{
    //    if (request == null)
    //    {
    //        throw new ArgumentNullException(nameof(request));
    //    }
    //    var handler = _serviceProvider.GetRequiredService<IApplicationHandler<IRequestDto>>();
    //    await _pipeline(new PipelineContext(handler, async (PipelineContext context) =>
    //    {
    //        await handler!.HandleAsync(request, cancellationToken);
    //    }, request, cancellationToken));
    //}

    //public async ValueTask<TResponse> ExecuteAsync<TResponse>(IRequestDto<TResponse> request, CancellationToken cancellationToken = default)
    //{
    //    if (request == null)
    //    {
    //        throw new ArgumentNullException(nameof(request));
    //    }
    //    var handlerType = typeof(IApplicationHandler<,>).MakeGenericType(request.GetType(),typeof(TResponse));
    //    var handler = _serviceProvider.GetRequiredService(handlerType);

    //    var context = new PipelineContext(handler, async (PipelineContext context) =>
    //    {
    //        context.ReturnValue = await handler.HandleAsync(request, cancellationToken);
    //    }, request, cancellationToken);
    //    await _pipeline(context);
    //    return (TResponse?)context.ReturnValue!;
    //}


    //public async ValueTask<TResponse> ExecuteAsync<THandler, TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
    //    where THandler : IApplicationHandler<TRequest, TResponse>
    //    where TRequest : IRequestDto<TResponse>
    //    where TResponse : notnull
    //{
    //    if (request == null)
    //    {
    //        throw new ArgumentNullException(nameof(request));
    //    }
    //    var handler = _serviceProvider.GetRequiredService<THandler>();
    //    var context = new PipelineContext(handler, async (PipelineContext context) =>
    //    {
    //        context.ReturnValue = await handler.HandleAsync(request, cancellationToken);
    //    }, request, cancellationToken);
    //    await _pipeline(context);
    //    return (TResponse?)context.ReturnValue!;
    //}

    //public async ValueTask ExecuteAsync<THandler>(IRequestDto request, CancellationToken cancellationToken = default)
    //    where THandler : IApplicationHandler<IRequestDto>
    //{
    //    if (request == null)
    //    {
    //        throw new ArgumentNullException(nameof(request));
    //    }
    //    if (request == null)
    //    {
    //        throw new ArgumentNullException(nameof(request));
    //    }
    //    var handler = _serviceProvider.GetRequiredService<THandler>();
    //    await _pipeline(new PipelineContext(handler, async (PipelineContext context) =>
    //    {
    //        await handler.HandleAsync(request, cancellationToken);
    //    }, request, cancellationToken));
    //}
}
