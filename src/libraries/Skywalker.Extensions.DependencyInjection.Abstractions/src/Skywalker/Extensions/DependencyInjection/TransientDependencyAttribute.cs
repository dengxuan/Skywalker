namespace Skywalker.Extensions.DependencyInjection;

/// <summary>
/// All classes usage this attribute are automatically registered to dependency injection as transient object.
/// This attribute cannot be multiple mark, can be inherited.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public class TransientDependencyAttribute : Attribute
{
}

