// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Extensions.DependencyInjection;

public abstract class Modular : IPreConfigureServices, IPostConfigureServices
{

    public static bool IsModular(Type type)
    {
        var typeInfo = type.GetTypeInfo();
        if (typeInfo.IsGenericType || typeInfo.IsAbstract || !typeInfo.IsClass)
        {
            return false;
        }
        return typeInfo.IsAssignableTo<IModular>();
    }

    internal static void CheckModuleType(Type moduleType)
    {
        if (!IsModular(moduleType))
        {
            throw new ArgumentException("Given type is not an modular: " + moduleType.AssemblyQualifiedName);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public virtual void PreConfigureServices(IServiceCollection services) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public virtual void PostConfigureServices(IServiceCollection services) { }
}
