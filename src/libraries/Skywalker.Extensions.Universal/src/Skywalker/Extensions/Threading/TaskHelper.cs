﻿using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Skywalker.Extensions.Threading;

/// <summary>
/// Provides some helper methods to work with async methods.
/// </summary>
public static class TaskHelper
{
    /// <summary>
    /// Checks if given method is an async method.
    /// </summary>
    /// <param name="method">A method to check</param>
    public static bool IsAsyncMethod(MethodInfo method)
    {
        return
            method.ReturnType == typeof(Task) ||
            method.ReturnType.GetTypeInfo().IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)
            ;
    }

    /// <summary>
    /// Runs a async method synchronously.
    /// </summary>
    /// <param name="func">A function that returns a result</param>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <returns>Result of the async operation</returns>
    public static TResult Run<TResult>(Func<Task<TResult>> func)
    {
        return Task.Run(func).Result;
    }

    /// <summary>
    /// Runs a async method synchronously.
    /// </summary>
    /// <param name="action">An async action</param>
    public static void Run(Func<Task> action)
    {
        Task.Run(action);
    }
}
