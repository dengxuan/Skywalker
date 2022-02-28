// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.DependencyInjection.Abstractions;

/// <summary>
/// 通过缓存解析的服务来提供服务。
/// 它缓存所有类型的服务，包括Transient。
/// 这个服务的生命周期是Scoped，应该被使用对于有限的范围。
/// </summary>

public interface ICachedServiceProvider : IScopedDependency, IServiceProvider
{
}
