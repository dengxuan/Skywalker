// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.DependencyInjection;


/// <summary>
/// 所有标记此属性的类都会自动作为范围对象注册到依赖注入容器中。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ScopedDependencyAttribute : Attribute { }
