// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;

namespace System;

public static class EnumExtensions
{
    /// <summary>
    /// 获取枚举的描述信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enum"></param>
    /// <returns></returns>
    public static string GetEnumDescription<T>(this T @enum) where T : Enum
    {
        var type = @enum.GetType();
        var name = Enum.GetName(type, @enum);
        if (name == null)
        {
            return string.Empty;
        }
        var field = type.GetField(name);
        if (field == null)
        {
            return string.Empty;
        }
        var descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>();
        if (descriptionAttribute == null)
        {
            return string.Empty;
        }

        return descriptionAttribute.Description;
    }

    /// <summary>
    /// 获取枚举的名字
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enum"></param>
    /// <returns></returns>
    public static string GetEnumName<T>(this T @enum) where T : Enum
    {
        var type = @enum.GetType();
        var name = Enum.GetName(type, @enum);
        return name ?? string.Empty;
    }
}
