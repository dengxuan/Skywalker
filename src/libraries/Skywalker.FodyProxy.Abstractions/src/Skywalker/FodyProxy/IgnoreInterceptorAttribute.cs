namespace Skywalker.FodyProxy;

/// <summary>
/// 被标记的类型或方法将忽略代码织入
/// </summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class IgnoreInterceptorAttribute : Attribute
{
    /// <summary>
    /// 忽略指定实现<see cref="IInterceptor"/>接口的织入类型，不传入忽略所有
    /// </summary>
    public Type[]? MoTypes { get; set; }
}
