// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.FodyProxy.Simple;

[Logging]
public class Service
{
    public static int Sync(string args)
    {
        return args.Length;
    }

    public static Task<int> Async(int id)
    {
        return Task.FromResult(id);
    }
}
