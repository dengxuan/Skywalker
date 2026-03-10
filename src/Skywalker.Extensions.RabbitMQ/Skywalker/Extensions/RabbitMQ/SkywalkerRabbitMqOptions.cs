using System.Text.Json.Serialization;

namespace Skywalker.Extensions.RabbitMQ;

/// <summary>
/// Options for RabbitMQ connections.
/// </summary>
public class SkywalkerRabbitMqOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Skywalker:RabbitMQ";

    /// <summary>
    /// Gets the RabbitMQ connections.
    /// </summary>
    [JsonIgnore]
    public RabbitMqConnections Connections { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SkywalkerRabbitMqOptions"/> class.
    /// </summary>
    public SkywalkerRabbitMqOptions()
    {
        Connections = [];
    }
}
