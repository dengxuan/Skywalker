// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.FodyProxy.Context;

namespace Skywalker.FodyProxy.Simple;

public class LoggingAttribute : InterceptorAttribute
{
    protected IServiceProvider? Service { get; set; }

    public override void OnEntry(MethodContext context)
    {
        Service!.GetService<Service>();
        Console.WriteLine("方法执行前");
    }

    public override void OnException(MethodContext context)
    {
        Console.WriteLine("方法执行异常,{0}", context.Exception);
    }

    public override void OnSuccess(MethodContext context)
    {
        Console.WriteLine("方法执行成功后");
    }

    public override void OnExit(MethodContext context)
    {
        Console.WriteLine("方法退出时，不论方法执行成功还是异常，都会执行");
    }
}
