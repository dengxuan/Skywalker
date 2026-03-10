// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.EventBus.Abstractions;

namespace Skywalker.EventBus;

[AttributeUsage(AttributeTargets.Class)]
public class QueueNameAttribute(string name) : Attribute, IQueueNameProvider
{
    public virtual string? Name { get; } = Check.NotNullOrWhiteSpace(name, nameof(name));

    public static string? GetNameOrDefault<TEvent>()
    {
        return GetNameOrDefault(typeof(TEvent));
    }

    public static string? GetNameOrDefault(Type eventType)
    {
        Check.NotNull(eventType, nameof(eventType));
        var nameProvider = eventType.GetCustomAttributes(true).OfType<IQueueNameProvider>().FirstOrDefault();
        if (nameProvider == null)
        {
            return null;
        }
        return nameProvider.GetName(eventType);
    }

    public string? GetName(Type eventType)
    {
        return Name;
    }
}
