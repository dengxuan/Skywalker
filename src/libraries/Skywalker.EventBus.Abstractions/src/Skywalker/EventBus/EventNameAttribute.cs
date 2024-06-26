﻿using Skywalker.EventBus.Abstractions;

namespace Skywalker.EventBus;

[AttributeUsage(AttributeTargets.Class)]
public class EventNameAttribute : Attribute, IEventNameProvider
{
    public virtual string Name { get; }

    public EventNameAttribute(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name));
    }

    public static string GetNameOrDefault<TEvent>()
    {
        return GetNameOrDefault(typeof(TEvent));
    }

    public static string GetNameOrDefault(Type eventType)
    {
        Check.NotNull(eventType, nameof(eventType));
        var nameProvider = eventType.GetCustomAttributes(true).OfType<IEventNameProvider>().FirstOrDefault();
        if (nameProvider == null)
        {
            return eventType.FullName!;
        }
        return nameProvider.GetName(eventType);
    }

    public string GetName(Type eventType)
    {
        return Name;
    }
}
