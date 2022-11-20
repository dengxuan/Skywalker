// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skywalker.Settings.Abstractions;
using Skywalker.Settings.Redis.Simple;
using Volo.Abp.Settings;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSettingsCore(options =>
{
    options.ValueProviders.Add<DefaultValueSettingValueProvider>();
    options.ValueProviders.Add<ConfigurationSettingValueProvider>();
    options.ValueProviders.Add<GlobalSettingValueProvider>();
    //options.ValueProviders.Add<UserSettingValueProvider>();
    options.DefinitionProviders.Add<SimpleSettingDefinitionProvider>();
}).AddRedisStore(options =>
{
    options.Configuration = "127.0.0.1:6379";
});
builder.Services.AddTransient<SimpleSettingDefinitionProvider>();
var app = builder.Build();
var settingProvider = app.Services.GetRequiredService<ISettingProvider>();

var username = await settingProvider.GetOrNullAsync("UserName");
Console.WriteLine(username);
await app.RunAsync();
