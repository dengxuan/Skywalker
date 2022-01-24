// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.DependencyInjection;

/// <summary>
/// All classes mark this attribute are automatically registered to dependency injection as singleton object.
/// This attribute cannot be multiple mark, can be inherited.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public sealed class SingletonDependencyAttribute : Attribute
{
}
