// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Settings;

/// <summary>
/// 
/// </summary>
public class SettingBuilder : ISettingBuilder
{
    /// <summary>
    /// 
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    internal SettingBuilder(IServiceCollection services) => Services = services;
}
