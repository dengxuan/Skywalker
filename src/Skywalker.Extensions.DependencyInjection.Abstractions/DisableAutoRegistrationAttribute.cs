using System;

namespace Skywalker.DependencyInjection;

/// <summary>
/// 禁用类的自动注册。
/// </summary>
/// <remarks>
/// 标记此特性的类即使实现了依赖注入接口，也不会被自动注册。
/// <para>
/// 使用示例：
/// <code>
/// [DisableAutoRegistration]
/// public class ManualService : IMyService, ITransientDependency
/// {
///     // 不会被自动注册，需要手动注册
/// }
/// </code>
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class DisableAutoRegistrationAttribute : Attribute
{
}
