// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.DependencyInjection.Interceptors;

namespace Skywalker.Extensions.DependencyInjection.Classes;

public interface IInterface1 : IIntercepted
{
    Task<int> GetIdAsync(int id);
}

public class Class1 : IInterface1
{
    public Task<int> GetIdAsync(int id)
    {
        return Task.FromResult(id);
    }
}

