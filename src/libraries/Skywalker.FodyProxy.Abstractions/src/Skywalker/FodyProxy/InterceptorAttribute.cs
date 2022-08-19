using Skywalker.FodyProxy.Context;

namespace Skywalker.FodyProxy;

/// <inheritdoc/>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Method)]
public abstract class InterceptorAttribute : Attribute, IInterceptor
{
    #region todo
    ///// <summary>
    ///// 默认构造方法
    ///// </summary>
    //protected MoAttribute() { }

    ///// <summary>
    ///// 通过构造方法传入的<see cref="AccessFlags"/>会覆盖类定义的<see cref="AccessFlags"/>
    ///// </summary>
    //protected MoAttribute(AccessFlags flags)
    //{
    //    Flags = flags;
    //}
    #endregion todo

    /// <inheritdoc/>
    public virtual AccessFlags Flags { get; }

    /// <inheritdoc/>
    public virtual void OnEntry(MethodContext context) { }

    /// <inheritdoc/>
    public virtual void OnException(MethodContext context) { }

    /// <inheritdoc/>
    public virtual void OnExit(MethodContext context) { }

    /// <inheritdoc/>
    public virtual void OnSuccess(MethodContext context) { }
}
