// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Ddd.Application.Dtos;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices((context, services) =>
{
    services.AddSkywalker();
});
var host = builder.Build();
var application = host.Services.GetRequiredService<IApplication>();
await application.ExecuteAsync(new EntityDto<long>(1));
await host.RunAsync();
