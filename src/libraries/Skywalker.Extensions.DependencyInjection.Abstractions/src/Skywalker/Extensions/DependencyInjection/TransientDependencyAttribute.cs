namespace Skywalker.Extensions.DependencyInjection;

/// <summary>
/// 所有标记此属性的类都会自动作为瞬时对象注册到依赖注入容器中。
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
public sealed class TransientDependencyAttribute : Attribute { }
