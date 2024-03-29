using Microsoft.Extensions.DependencyInjection;
using Skywalker.EventBus.Abstractions;

namespace Skywalker.EventBus;

/// <summary>
/// This <see cref="IEventHandlerFactory"/> implementation is used to get/release
/// handlers using Ioc.
/// </summary>
public class IocEventHandlerFactory : IEventHandlerFactory, IDisposable
{
    private bool _disposedValue;

    public Type HandlerType { get; }

    protected IServiceScopeFactory ScopeFactory { get; }

    public IocEventHandlerFactory(IServiceScopeFactory scopeFactory, Type handlerType)
    {
        ScopeFactory = scopeFactory;
        HandlerType = handlerType;
    }

    /// <summary>
    /// Resolves handler object from Ioc container.
    /// </summary>
    /// <returns>Resolved handler object</returns>
    public IEventHandlerDisposeWrapper GetHandler()
    {
        var scope = ScopeFactory.CreateScope();
        var eventHandler = (IEventHandler)scope.ServiceProvider.GetRequiredService(HandlerType);
        return new EventHandlerDisposeWrapper(eventHandler, () => scope.Dispose());
    }

    public bool IsInFactories(List<IEventHandlerFactory> handlerFactories)
    {
        return handlerFactories
            .OfType<IocEventHandlerFactory>()
            .Any(f => f.HandlerType == HandlerType);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
            }

            _disposedValue = true;
        }
    }

    ~IocEventHandlerFactory()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
