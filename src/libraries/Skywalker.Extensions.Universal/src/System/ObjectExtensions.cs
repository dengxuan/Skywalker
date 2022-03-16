using System.ComponentModel;
using System.Globalization;

namespace System;

/// <summary>
/// Extension methods for all objects.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Used to simplify and beautify casting an object to a type. 
    /// </summary>
    /// <typeparam name="T">Type to be casted</typeparam>
    /// <param name="obj">Object to cast</param>
    /// <returns>Casted object</returns>
    public static T As<T>(this object obj) where T : class
    {
        return (T)obj;
    }

    /// <summary>
    /// Converts given object to a value type using <see cref="Convert.ChangeType(object,TypeCode)"/> method.
    /// </summary>
    /// <param name="object">Object to be converted</param>
    /// <typeparam name="T">Type of the target object</typeparam>
    /// <returns>Converted object</returns>
    public static T To<T>(this object @object) where T : struct
    {
        if (typeof(T) == typeof(Guid))
        {
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(@object?.ToString()!)!;
        }

        return (T)Convert.ChangeType(@object, typeof(T), CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Check if an item is in a list.
    /// </summary>
    /// <param name="item">Item to check</param>
    /// <param name="list">List of items</param>
    /// <typeparam name="T">Type of the items</typeparam>
    public static bool IsIn<T>(this T item, params T[] list)
    {
        return list.Contains(item);
    }


    ///// <summary>
    ///// Executes given <paramref name="action"/> by locking given <paramref name="source"/> object.
    ///// </summary>
    ///// <param name="source">Source object (to be locked)</param>
    ///// <param name="action">Action (to be executed)</param>
    public static void Locking(this object source, Action action)
    {
        lock (source)
        {
            action();
        }
    }

    /// <summary>
    /// Executes given <paramref name="action"/> by locking given <paramref name="source"/> object.
    /// </summary>
    /// <typeparam name="T">Type of the object (to be locked)</typeparam>
    /// <param name="source">Source object (to be locked)</param>
    /// <param name="action">Action (to be executed)</param>
    public static void Locking<T>(this T source, Action<T> action) where T : class
    {
        lock (source)
        {
            action(source);
        }
    }

    /// <summary>
    /// Executes given <paramref name="func"/> and returns it's value by locking given <paramref name="source"/> object.
    /// </summary>
    /// <typeparam name="TResult">Return type</typeparam>
    /// <param name="source">Source object (to be locked)</param>
    /// <param name="func">Function (to be executed)</param>
    /// <returns>Return value of the <paramref name="func"/></returns>
    public static TResult Locking<TResult>(this object source, Func<TResult> func)
    {
        lock (source)
        {
            return func();
        }
    }

    /// <summary>
    /// Executes given <paramref name="func"/> and returns it's value by locking given <paramref name="source"/> object.
    /// </summary>
    /// <typeparam name="T">Type of the object (to be locked)</typeparam>
    /// <typeparam name="TResult">Return type</typeparam>
    /// <param name="source">Source object (to be locked)</param>
    /// <param name="func">Function (to be executed)</param>
    /// <returns>Return value of the <paramnref name="func"/></returns>
    public static TResult Locking<T, TResult>(this T source, Func<T, TResult> func) where T : class
    {
        lock (source)
        {
            return func(source);
        }
    }
}
