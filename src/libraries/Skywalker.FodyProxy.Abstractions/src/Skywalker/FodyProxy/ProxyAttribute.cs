namespace Skywalker.FodyProxy;

/// <summary>
/// 使用一个已有Attribute作为代理类型进行织入
/// </summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
public class ProxyAttribute : Attribute
{
    /// <summary>
    /// </summary>
    public ProxyAttribute(Type originAttributeType, Type targetAttribtueType)
    {
        OriginAttributeType = originAttributeType;
        TargetAttribtueType = targetAttribtueType;
    }

    /// <summary>
    /// 被代理的Attribute类型
    /// </summary>
    public Type OriginAttributeType { get; set; }

    /// <summary>
    /// 继承自<see cref="InterceptorAttribute"/>的代理Attribute
    /// </summary>
    public Type TargetAttribtueType { get; set; }
}
