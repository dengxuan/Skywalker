// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.DependencyInjection.Interceptors;

namespace Skywalker.Extensions.DependencyInjection.Classes;

public interface IInterface1 : IIntercepted
{
    Task<int> GetIdAsync(int id);
}

public interface IInterface2: IIntercepted
{
    Task SetIdAsync(int id);
}

public class Class1 : IInterface1, IInterface2
{
    public Task<int> GetIdAsync(int id)
    {
        return Task.FromResult(id);
    }

    public Task SetIdAsync(int id)
    {
        Console.WriteLine(id);
        return Task.CompletedTask;
    }
}

