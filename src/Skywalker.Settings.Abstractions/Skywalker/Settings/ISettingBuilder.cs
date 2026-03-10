// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Settings;

/// <summary>
/// 
/// </summary>
public interface ISettingBuilder
{
    /// <summary>
    /// 
    /// </summary>
    IServiceCollection Services { get; }
}
