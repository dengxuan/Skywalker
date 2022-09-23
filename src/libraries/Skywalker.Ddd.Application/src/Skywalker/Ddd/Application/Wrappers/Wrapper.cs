namespace MediatR.Wrappers;

using System;
using Microsoft.Extensions.DependencyInjection;

public abstract class Wrapper
{
    private readonly IServiceProvider _services;

    protected Wrapper(IServiceProvider services) => _services = services;

    protected THandler GetHandler<THandler>() where THandler : notnull
    {
        return _services.GetRequiredService<THandler>();
    }
}
