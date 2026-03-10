using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Skywalker.EventBus.Abstractions;

namespace Skywalker.EventBus.Local;

/// <summary>
/// Options for the local event bus.
/// </summary>
public class LocalEventBusOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Skywalker:EventBus:Local";

    /// <summary>
    /// Gets the list of event handler types.
    /// This property is not bound from configuration.
    /// </summary>
    [JsonIgnore]
    public List<Type> Handlers { get; } = [];

    /// <summary>
    /// Gets or sets the channel capacity for buffering events.
    /// Default is 1000.
    /// </summary>
    [Range(1, 100000)]
    public int ChannelCapacity { get; set; } = 1000;

    /// <summary>
    /// Adds an event handler type.
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    public void AddEventHandler<THandler>() where THandler : IEventHandler
    {
        var implemented = typeof(THandler).GetInterfaces().Any(i =>
        {
            return i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>);
        });

        if (!implemented)
        {
            throw new InvalidOperationException($"{typeof(THandler).FullName} must implement IEventHandler<TEvent>");
        }

        Handlers.Add(typeof(THandler));
    }
}

