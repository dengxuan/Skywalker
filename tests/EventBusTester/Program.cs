// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using EventBusTester;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skywalker.EventBus.Abstractions;
using Skywalker.Extensions.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<SkywalkerRabbitMqOptions>(builder.Configuration.GetSection("Rabbitmq"));
var c = builder.Configuration.GetConnectionString("RabbitMQ");
builder.Services.AddEventBus(builder =>
{
    builder.Handlers.Add<TrxHandler>();
    builder.Handlers.Add<UsdtHandler>();
    builder.UseRabbitMQ(options =>
    {
        options.ConnectionName = "RabbitMQ";
        options.ExchangeName = "Skywalker";
        options.ClientName = "Skywalker";
        options.PrefetchCount = 10;
    });
});
builder.Services.AddJsonSerializer();
builder.Services.AddTransient<UsdtHandler>();
builder.Services.AddTransient<TrxHandler>();
var app = builder.Build();
var eventBus = app.Services.GetRequiredService<IEventBus>();

await eventBus.PublishAsync(new TrxEto("1", "Gordon", 120));

await app.RunAsync();
