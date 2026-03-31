
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 显式指定要暴露的服务接口。
/// </summary>
/// <remarks>
/// 使用此特性可以覆盖自动发现的服务接口。
/// <para>
/// 使用示例：
/// <code>
/// [ExposeServices(typeof(IEmailService), typeof(INotificationService))]
/// public class EmailService : IEmailService, INotificationService, IInternalService
/// {
///     // 只会注册 IEmailService 和 INotificationService，不会注册 IInternalService
/// }
/// </code>
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class ExposeServicesAttribute : Attribute
{
    /// <summary>
    /// 获取要暴露的服务类型列表。
    /// </summary>
    public Type[] ServiceTypes { get; }

    /// <summary>
    /// 获取或设置是否包含自身类型。
    /// </summary>
    /// <remarks>
    /// 默认为 <c>false</c>。如果设置为 <c>true</c>，则实现类自身也会被注册为服务。
    /// </remarks>
    public bool IncludeSelf { get; set; }

    /// <summary>
    /// 获取或设置是否包含默认服务（自动发现的接口）。
    /// </summary>
    /// <remarks>
    /// 默认为 <c>false</c>。如果设置为 <c>true</c>，则除了显式指定的接口外，自动发现的接口也会被注册。
    /// </remarks>
    public bool IncludeDefaults { get; set; }

    /// <summary>
    /// 初始化 <see cref="ExposeServicesAttribute"/> 类的新实例。
    /// </summary>
    /// <param name="serviceTypes">要暴露的服务类型。</param>
    public ExposeServicesAttribute(params Type[] serviceTypes)
    {
        ServiceTypes = serviceTypes ?? Array.Empty<Type>();
    }
}
