// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;

namespace Skywalker.ObjectMapping.AutoMapper;

/// <summary>
/// AutoMapper 配置选项
/// </summary>
public class AutoMapperOptions
{
    /// <summary>
    /// 要扫描 Profile 的程序集列表
    /// </summary>
    public List<Assembly> ProfileAssemblies { get; } = new();

    /// <summary>
    /// 添加要扫描的程序集
    /// </summary>
    /// <param name="assembly">程序集</param>
    /// <returns>当前选项实例</returns>
    public AutoMapperOptions AddAssembly(Assembly assembly)
    {
        if (!ProfileAssemblies.Contains(assembly))
        {
            ProfileAssemblies.Add(assembly);
        }
        return this;
    }

    /// <summary>
    /// 添加包含指定类型的程序集
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>当前选项实例</returns>
    public AutoMapperOptions AddAssembly<T>()
    {
        return AddAssembly(typeof(T).Assembly);
    }
}
