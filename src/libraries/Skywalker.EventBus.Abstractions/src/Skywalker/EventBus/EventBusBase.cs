using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skywalker.EventBus.Abstractions;
using Skywalker.Extensions.Collections.Generic;

namespace Skywalker.EventBus;

public abstract class EventBusBase : BackgroundService, IEventBus
{
    protected IServiceScopeFactory ServiceScopeFactory { get; }

    protected EventBusBase(IServiceScopeFactory serviceScopeFactory)
    {
        ServiceScopeFactory = serviceScopeFactory;
    }

    /// <inheritdoc/>
    public virtual IDisposable Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
    {
        return Subscribe(typeof(TEvent), new ActionEventHandler<TEvent>(action));
    }

    /// <inheritdoc/>
    public virtual IDisposable Subscribe<TEvent, THandler>()
        where TEvent : class
        where THandler : IEventHandler, new()
    {
        return Subscribe(typeof(TEvent), new TransientEventHandlerFactory<THandler>());
    }

    /// <inheritdoc/>
    public virtual IDisposable Subscribe(Type eventType, IEventHandler handler)
    {
        return Subscribe(eventType, new SingleInstanceHandlerFactory(handler));
    }

    /// <inheritdoc/>
    public virtual IDisposable Subscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
    {
        return Subscribe(typeof(TEvent), factory);
    }

    public abstract IDisposable Subscribe(Type eventType, IEventHandlerFactory factory);

    public virtual IDisposable Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
    {
        return Subscribe(typeof(TEvent), handler);
    }

    public abstract void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class;

    /// <inheritdoc/>
    public virtual void Unsubscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
    {
        Unsubscribe(typeof(TEvent), handler);
    }

    public abstract void Unsubscribe(Type eventType, IEventHandler handler);

    /// <inheritdoc/>
    public virtual void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
    {
        Unsubscribe(typeof(TEvent), factory);
    }

    public abstract void Unsubscribe(Type eventType, IEventHandlerFactory factory);

    /// <inheritdoc/>
    public virtual void UnsubscribeAll<TEvent>() where TEvent : class
    {
        UnsubscribeAll(typeof(TEvent));
    }

    /// <inheritdoc/>
    public abstract void UnsubscribeAll(Type eventType);

    /// <inheritdoc/>
    public virtual Task PublishAsync<TEvent>(TEvent eventArgs) where TEvent : class
    {
        return PublishAsync(typeof(TEvent), eventArgs);
    }

    /// <inheritdoc/>
    public abstract Task PublishAsync(Type eventType, object eventArgs);

    public virtual async Task TriggerHandlersAsync(Type eventType, object eventArgs)
    {
        var exceptions = new List<Exception>();

        await TriggerHandlersAsync(eventType, eventArgs, exceptions);

        if (exceptions.Count != 0)
        {
            if (exceptions.Count == 1)
            {
                exceptions[0].ReThrow();
            }

            throw new AggregateException("More than one error has occurred while triggering the event: " + eventType, exceptions);
        }
    }

    protected virtual async Task TriggerHandlersAsync(Type eventType, object eventArgs, List<Exception> exceptions)
    {
        await new SynchronizationContextRemover();

        foreach (var handlerFactories in GetHandlerFactories(eventType))
        {
            foreach (var handlerFactory in handlerFactories.EventHandlerFactories)
            {
                await TriggerHandlerAsync(handlerFactory, handlerFactories.EventType, eventArgs, exceptions);
            }
        }

        //Implements generic argument inheritance. See IEventDataWithInheritableGenericArgument
        if (eventType.GetTypeInfo().IsGenericType &&
            eventType.GetGenericArguments().Length == 1 &&
            typeof(IEventDataWithInheritableGenericArgument).IsAssignableFrom(eventType))
        {
            var genericArg = eventType.GetGenericArguments()[0];
            var baseArg = genericArg.GetTypeInfo().BaseType;
            if (baseArg != null)
            {
                var baseEventType = eventType.GetGenericTypeDefinition().MakeGenericType(baseArg);
                var constructorArgs = ((IEventDataWithInheritableGenericArgument)eventArgs).GetConstructorArgs();
                var baseEventData = Activator.CreateInstance(baseEventType, constructorArgs);
                await PublishAsync(baseEventType, baseEventData!);
            }
        }
    }

    protected virtual void SubscribeHandlers(ITypeList<IEventHandler> handlers)
    {
        foreach (var handler in handlers)
        {
            var interfaces = handler.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (!typeof(IEventHandler).GetTypeInfo().IsAssignableFrom(@interface))
                {
                    continue;
                }

                var genericArgs = @interface.GetGenericArguments();
                if (genericArgs.Length == 1)
                {
                    Subscribe(genericArgs[0], new IocEventHandlerFactory(ServiceScopeFactory, handler));
                }
            }
        }
    }

    protected virtual void UnsubscribeHandlers(ITypeList<IEventHandler> handlers)
    {
        foreach (var handler in handlers)
        {
            var interfaces = handler.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (!typeof(IEventHandler).GetTypeInfo().IsAssignableFrom(@interface))
                {
                    continue;
                }

                var genericArgs = @interface.GetGenericArguments();
                if (genericArgs.Length == 1)
                {
                    Unsubscribe(genericArgs[0], new IocEventHandlerFactory(ServiceScopeFactory, handler));
                }
            }
        }
    }

    protected abstract IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType);

    protected virtual async Task TriggerHandlerAsync(IEventHandlerFactory asyncHandlerFactory, Type eventType, object eventData, List<Exception> exceptions)
    {
        using var eventHandlerWrapper = asyncHandlerFactory.GetHandler();
        try
        {
            var handlerType = eventHandlerWrapper.EventHandler?.GetType();
            if (handlerType == null)
            {
                //Todo throw exception
                return;
            }

            if (ReflectionHelper.IsAssignableToGenericType(handlerType, typeof(IEventHandler<>)))
            {
                var method = typeof(IEventHandler<>)
                    .MakeGenericType(eventType)
                    .GetMethod(nameof(IEventHandler<object>.HandleEventAsync), new[] { eventType });

                await (Task)method?.Invoke(eventHandlerWrapper!.EventHandler, new[] { eventData })!;
            }
            else
            {
                throw new NotImplementedException("The object instance is not an event handler. Object type: " + handlerType.AssemblyQualifiedName);
            }
        }
        catch (TargetInvocationException ex)
        {
            if (ex.InnerException != null)
            {
                exceptions.Add(ex.InnerException);
            }
        }
        catch (Exception ex)
        {
            exceptions.Add(ex);
        }
    }

    protected class EventTypeWithEventHandlerFactories
    {
        public Type EventType { get; }

        public List<IEventHandlerFactory> EventHandlerFactories { get; }

        public EventTypeWithEventHandlerFactories(Type eventType, List<IEventHandlerFactory> eventHandlerFactories)
        {
            EventType = eventType;
            EventHandlerFactories = eventHandlerFactories;
        }
    }

    // Reference from
    // https://blogs.msdn.microsoft.com/benwilli/2017/02/09/an-alternative-to-configureawaitfalse-everywhere/
    protected struct SynchronizationContextRemover : INotifyCompletion
    {
        public bool IsCompleted
        {
            get { return SynchronizationContext.Current == null; }
        }

        public void OnCompleted(Action continuation)
        {
            var prevContext = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(null);
                continuation();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevContext);
            }
        }

        public SynchronizationContextRemover GetAwaiter()
        {
            return this;
        }

        public void GetResult()
        {
        }
    }
}
