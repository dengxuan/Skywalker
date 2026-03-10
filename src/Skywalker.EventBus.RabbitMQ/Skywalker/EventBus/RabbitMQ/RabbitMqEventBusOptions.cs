// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Skywalker.Extensions.RabbitMQ;

namespace Skywalker.EventBus.RabbitMQ;

/// <summary>
/// Options for RabbitMQ event bus.
/// </summary>
public class RabbitMqEventBusOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Skywalker:EventBus:RabbitMQ";

    /// <summary>
    /// Default exchange type.
    /// </summary>
    public const string DefaultExchangeType = RabbitMqConsts.ExchangeTypes.Direct;

    /// <summary>
    /// Gets or sets the connection name.
    /// </summary>
    public string? ConnectionName { get; set; }

    /// <summary>
    /// Gets or sets the client name.
    /// </summary>
    [Required]
    public string ClientName { get; set; } = default!;

    /// <summary>
    /// Gets or sets the exchange name.
    /// </summary>
    [Required]
    public string ExchangeName { get; set; } = default!;

    /// <summary>
    /// Gets or sets the exchange type (direct, topic, fanout, headers).
    /// </summary>
    public string? ExchangeType { get; set; }

    /// <summary>
    /// Gets or sets the prefetch count.
    /// </summary>
    [Range(1, 65535)]
    public ushort? PrefetchCount { get; set; }

    /// <summary>
    /// Gets or sets the queue arguments.
    /// </summary>
    [JsonIgnore]
    public IDictionary<string, object> QueueArguments { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets or sets the exchange arguments.
    /// </summary>
    [JsonIgnore]
    public IDictionary<string, object> ExchangeArguments { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets the exchange type or default.
    /// </summary>
    public string GetExchangeTypeOrDefault()
    {
        return string.IsNullOrEmpty(ExchangeType)
            ? DefaultExchangeType
            : ExchangeType!;
    }
}
