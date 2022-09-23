// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skywalker.Extensions.DependencyInjection;
using Skywalker.Extensions.DependencyInjection.Abstractions;
using Skywalker.Extensions.DependencyInjection.Classes;
using Skywalker.Extensions.DependencyInjection.Generators;
using Skywalker.Extensions.DependencyInjection.Interceptors;
using Skywalker.Extensions.DependencyInjection.Interceptors.Abstractions;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(configure =>
{
    configure.AddSingleton<IInterceptorChainBuilder, InterceptorChainBuilder>();
    configure.AddSingleton<IObjectAccessor<InterceptorDelegate>>(sp =>
    {
        var chain = sp.GetRequiredService<IInterceptorChainBuilder>();
       return new ObjectAccessor<InterceptorDelegate>(chain.Build());
    });
    configure.AddSkywalker();
});

var app = builder.Build();
app.UseAspects(chain =>
{
    chain.Use((next) => async context =>
    {
        Console.WriteLine("AAA");
        await next(context);
        Console.WriteLine("BBB");
    });
});
var i = app.Services.GetRequiredService<IInterface1>();
var j = app.Services.GetRequiredService<IInterface2>();
await j.SetIdAsync(100);
Console.WriteLine(await i.GetIdAsync(10));
await app.RunAsync();
