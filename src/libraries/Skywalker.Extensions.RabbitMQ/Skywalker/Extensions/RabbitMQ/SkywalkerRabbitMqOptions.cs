namespace Skywalker.Extensions.RabbitMQ;

public class SkywalkerRabbitMqOptions
{
    public RabbitMqConnections Connections { get; }

    public SkywalkerRabbitMqOptions()
    {
        Connections = [];
    }
}
