namespace Skywalker.FodyProxy;

/// <summary>
/// 多互斥类型，与<see cref="IRepulsionsProxy{TMo, TRepulsion}"/>配合使用
/// </summary>
public abstract class Repulsion
{
    /// <summary>
    /// 类型必须继承自<see cref="IInterceptor"/>，同时实现该类时，该字段必须一次性初始化，不能包含逻辑处理
    /// </summary>
    /// <example>
    /// Repulsions = new [] { typeof(Abc), typeof(Bcd) };
    /// </example>
    public abstract Type[] Repulsions { get; }
}
