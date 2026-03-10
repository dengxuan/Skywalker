using System;

namespace Skywalker;

/// <summary>
/// 标记程序集为 Skywalker 模块，指定模块名称。
/// </summary>
/// <remarks>
/// 此特性用于 Source Generator 在编译时：
/// <list type="number">
/// <item><description>自动生成 <c>Add{ModuleName}()</c> 扩展方法（如果不存在）</description></item>
/// <item><description>在应用程序中聚合所有模块，生成 <c>AddSkywalker()</c> 方法</description></item>
/// </list>
/// <para>
/// 使用示例：
/// <code>
/// [assembly: SkywalkerModule("Security")]  // 生成 AddSecurity()
/// [assembly: SkywalkerModule("RedisCaching")]  // 生成 AddRedisCaching()
/// </code>
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class SkywalkerModuleAttribute : Attribute
{
    /// <summary>
    /// 获取模块名称。
    /// </summary>
    /// <remarks>
    /// 模块名称将用于生成 <c>Add{ModuleName}()</c> 方法。
    /// 例如：模块名称为 "Security"，则生成 <c>AddSecurity()</c> 方法。
    /// </remarks>
    public string ModuleName { get; }

    /// <summary>
    /// 初始化 <see cref="SkywalkerModuleAttribute"/> 类的新实例。
    /// </summary>
    /// <param name="moduleName">模块名称，用于生成 Add{ModuleName}() 方法。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="moduleName"/> 为 <c>null</c> 或空字符串时抛出。</exception>
    public SkywalkerModuleAttribute(string moduleName)
    {
        if (string.IsNullOrWhiteSpace(moduleName))
            throw new ArgumentNullException(nameof(moduleName));
        ModuleName = moduleName;
    }
}
