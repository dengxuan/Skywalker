namespace Skywalker.FodyProxy;

/// <summary>
/// 对实现该接口的类型，使用<typeparamref name="T"/>进行代码织入
/// </summary>
/// <typeparam name="T">实现<see cref="IInterceptor"/>的具体类型</typeparam>
public interface IProxy<in T> where T : IInterceptor, new()
{
}

/// <summary>
/// 对于实现该接口的类型，使用<typeparamref name="TMo"/>进行代码织入
/// </summary>
/// <typeparam name="TMo">实现<see cref="IInterceptor"/>的具体类型</typeparam>
/// <typeparam name="TRepulsion">继承自<see cref="IInterceptor"/>，与<typeparamref name="TMo"/>互斥，根据优先级关系同时仅一个生效</typeparam>
public interface IProxy<in TMo, in TRepulsion> where TMo : IInterceptor, new() where TRepulsion : IInterceptor
{
}

/// <summary>
/// 对于实现该接口的类型，使用<typeparamref name="TMo"/>进行代码织入
/// </summary>
/// <typeparam name="TMo">实现<see cref="IInterceptor"/>的具体类型</typeparam>
/// <typeparam name="TRepulsion">
/// 继承自<see cref="Repulsion"/>，其<see cref="Repulsion.Repulsions"/>定义了多个与<typeparamref name="TMo"/>互斥类型，根据优先级关系同时仅一个生效
/// </typeparam>
public interface IRepulsionsProxy<in TMo, in TRepulsion> where TMo : IInterceptor, new() where TRepulsion : Repulsion
{
}
