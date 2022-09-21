// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skywalker.Aspects;
using Skywalker.Extensions.DependencyInjection.Classes;
using Skywalker.Extensions.DependencyInjection.Generators;
using Skywalker.Extensions.DependencyInjection.Interceptors;
using Skywalker.Extensions.DependencyInjection.Interceptors.Abstractions;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(configure =>
{
    configure.AddSingleton<IInterceptorChainBuilder, InterceptorChainBuilder>();
    configure.AddTransient<IInterface1, Class1>().DecorateWithDispatchProxy<IInterface1, Class1Proxy>();
});
var app = builder.Build();
var chain = app.Services.GetRequiredService<IInterceptorChainBuilder>();
chain.Use((next) =>
{
    Console.WriteLine("AAA");
    return next;
});
var i = app.Services.GetRequiredService<IInterface1>();
Console.WriteLine(await i.GetIdAsync(10));
await app.RunAsync();
