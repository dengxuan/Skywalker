using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Extensions.Options;
using Skywalker.DependencyInjection;
using Skywalker.EventBus.Abstractions;

namespace Skywalker.EventBus.Local;

/// <summary>
/// Local event bus implementation using System.Threading.Channels.
/// </summary>
public class LocalChannelEventBus : EventBusBase, ILocalEventBus, IAsyncDisposable, ISingletonDependency
{
    private readonly IEventHandlerFactory _handlerFactory;
    private readonly IEventHandlerInvoker _handlerInvoker;
    private readonly ConcurrentDictionary<Type, List<Type>> _handlerTypes;
    private readonly Channel<EventMessage> _channel;
    private readonly Task _processingTask;
    private readonly CancellationTokenSource _cts;
    private int _disposed;

    public LocalChannelEventBus(
        IEventHandlerFactory handlerFactory,
        IEventHandlerInvoker handlerInvoker,
        IOptions<LocalEventBusOptions> options)
    {
        _handlerFactory = handlerFactory;
        _handlerInvoker = handlerInvoker;
        _handlerTypes = new ConcurrentDictionary<Type, List<Type>>();
        _cts = new CancellationTokenSource();

        _channel = Channel.CreateBounded<EventMessage>(new BoundedChannelOptions(options.Value.ChannelCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });

        foreach (var handlerType in options.Value.Handlers)
        {
            RegisterHandler(handlerType);
        }

        _processingTask = StartProcessingAsync(_cts.Token);
    }

    /// <inheritdoc/>
    public override async Task PublishAsync(Type eventType, object eventArgs)
    {
        await _channel.Writer.WriteAsync(new EventMessage(eventType, eventArgs));
    }

    /// <inheritdoc/>
    public void Subscribe<TEvent, THandler>()
        where TEvent : class
        where THandler : IEventHandler<TEvent>
    {
        Subscribe(typeof(TEvent), typeof(THandler));
    }

    /// <inheritdoc/>
    public void Subscribe(Type eventType, Type handlerType)
    {
        var handlers = _handlerTypes.GetOrAdd(eventType, _ => new List<Type>());
        lock (handlers)
        {
            if (!handlers.Contains(handlerType))
            {
                handlers.Add(handlerType);
            }
        }
    }

    /// <inheritdoc/>
    public void Unsubscribe<TEvent, THandler>()
        where TEvent : class
        where THandler : IEventHandler<TEvent>
    {
        Unsubscribe(typeof(TEvent), typeof(THandler));
    }

    /// <inheritdoc/>
    public void Unsubscribe(Type eventType, Type handlerType)
    {
        if (_handlerTypes.TryGetValue(eventType, out var handlers))
        {
            lock (handlers)
            {
                handlers.Remove(handlerType);
            }
        }
    }

    private async Task StartProcessingAsync(CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var message in _channel.Reader.ReadAllAsync(cancellationToken))
            {
                // Check if we're being disposed before processing
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var handlers = GetHandlers(message.EventType);
                foreach (var handlerType in handlers)
                {
                    // Check again before each handler invocation
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    try
                    {
                        var handler = _handlerFactory.GetHandler(handlerType);
                        await _handlerInvoker.InvokeAsync(handler, message.EventData, message.EventType);
                    }
                    catch (ObjectDisposedException)
                    {
                        // Service provider was disposed during shutdown, ignore
                        break;
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when disposing
        }
        catch (ObjectDisposedException)
        {
            // Service provider was disposed during shutdown, ignore
        }
    }

    private void RegisterHandler(Type handlerType)
    {
        var interfaces = handlerType.GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>));

        foreach (var @interface in interfaces)
        {
            var eventType = @interface.GetGenericArguments()[0];
            Subscribe(eventType, handlerType);
        }
    }

    private IEnumerable<Type> GetHandlers(Type eventType)
    {
        var handlers = new List<Type>();

        if (_handlerTypes.TryGetValue(eventType, out var directHandlers))
        {
            lock (directHandlers)
            {
                handlers.AddRange(directHandlers);
            }
        }

        foreach (var kvp in _handlerTypes)
        {
            if (kvp.Key != eventType && kvp.Key.IsAssignableFrom(eventType))
            {
                lock (kvp.Value)
                {
                    handlers.AddRange(kvp.Value);
                }
            }
        }

        return handlers.Distinct();
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1)
        {
            return;
        }

        await _cts.CancelAsync();
        _channel.Writer.TryComplete();
        await _processingTask;
        _cts.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly record struct EventMessage(Type EventType, object EventData);
}

