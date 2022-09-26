using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Application.Wrappers;

public abstract class Wrapper
{
    private readonly IServiceProvider _services;

    protected Wrapper(IServiceProvider services) => _services = services;

    protected THandler GetHandler<THandler>() where THandler : notnull
    {
        return _services.GetRequiredService<THandler>();
    }
}
