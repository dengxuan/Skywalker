//// Licensed to the Gordon under one or more agreements.
//// Gordon licenses this file to you under the MIT license.

//using System;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using System.Text;
//using Skywalker.Ddd.Application.Abstractions;
//using Skywalker.Ddd.Application.Dtos.Abstractions;
//using Skywalker.Ddd.Application.Pipeline;

//namespace Skywalker.Ddd.Application;

//internal abstract class DefaultApplicationHandler
//{
//    protected static THandler GetHandler<THandler>(ApplicationHandlerFactory factory)
//    {
//        THandler handler;

//        try
//        {
//            handler = factory.GetInstance<THandler>();
//        }
//        catch (Exception e)
//        {
//            throw new InvalidOperationException($"Error constructing handler for request of type {typeof(THandler)}. Register your handlers with the container. See the samples in GitHub for examples.", e);
//        }

//        if (handler == null)
//        {
//            throw new InvalidOperationException($"Handler was not found for request of type {typeof(THandler)}. Register your handlers with the container. See the samples in GitHub for examples.");
//        }

//        return handler;
//    }

//}

//internal class DefaultApplicationHandler<TResponse> : DefaultApplicationHandler
//{
//    public async Task<object?> Handle(object request, CancellationToken cancellationToken, ApplicationHandlerFactory serviceFactory)
//    {
//        return await Handle((IApplicationHandler<IRequestDto<TResponse>, TResponse>)request, cancellationToken, serviceFactory).ConfigureAwait(false);
//    }
//}

//internal class DefaultApplicationHandler<TRequest, TResponse> : DefaultApplicationHandler<TRequest> where TRequest : IRequestDto<TResponse>
//{
//    private readonly InterceptDelegate _pipeline;

//    public async ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
//    {
//        var handler = GetHandler<IApplicationHandler<TRequest, TResponse>>(serviceFactory);
//        var context = new PipelineContext(handler, async (PipelineContext context) =>
//        {
//            context.ReturnValue = await handler.HandleAsync(request, cancellationToken);
//        }, request, cancellationToken);
//        await _pipeline(context);
//        return (TResponse)context.ReturnValue!;
//    }
//}
