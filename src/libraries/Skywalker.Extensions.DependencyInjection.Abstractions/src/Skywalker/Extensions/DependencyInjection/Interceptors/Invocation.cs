//// Licensed to the .NET Foundation under one or more agreements.
//// The .NET Foundation licenses this file to you under the MIT license.

//using System.Reflection;
//using Microsoft.Extensions.DependencyInjection;
//using Skywalker.Extensions.DependencyInjection.Interceptors.Abstractions;

//namespace Skywalker.Extensions.DependencyInjection.Interceptors;

///// <summary>
///// DispatchProxy provides a mechanism for the instantiation of proxy objects and handling of
///// their method dispatch.
///// </summary>
//public sealed class Invocation
//{
//    private readonly IServiceProvider _service;
//    private readonly InterceptorDelegate _interceptor;

//    internal Invocation(IServiceProvider service)
//    {
//        _service = service;
//        var chainBuilder = _service.GetRequiredService<IInterceptorChainBuilder>();
//        _interceptor = chainBuilder.Build();
//    }


//    private Task Warper(InvocationContext context)
//    {
//        context.ReturnValue = context.Method.Invoke(context.Target, context.Arguments);
//        return Task.CompletedTask;
//    }

//    ///// <summary>
//    ///// Whenever any method on the generated proxy type is called, this method
//    ///// will be invoked to dispatch control.
//    ///// </summary>
//    ///// <param name="targetMethod">The method the caller invoked</param>
//    ///// <param name="args">The arguments the caller passed to the method</param>
//    ///// <returns>The object to return to the caller, or <c>null</c> for void methods</returns>
//    public object? Invoke(object target, MethodInfo method, object[] args)
//    {
//        var context = new DefaultInvocationContext(_service, method, target, args);
//        _interceptor(Warper)(context);
//        return context.ReturnValue;
//    }
//}
