// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace System.Threading.Tasks;

public static class TaskCache
{
    public static Task<bool> TrueResult { get; }
    public static Task<bool> FalseResult { get; }

    static TaskCache()
    {
        TrueResult = Task.FromResult(true);
        FalseResult = Task.FromResult(false);
    }
}
