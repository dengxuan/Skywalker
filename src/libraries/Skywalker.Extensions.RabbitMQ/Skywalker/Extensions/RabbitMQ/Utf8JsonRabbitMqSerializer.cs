using System.Text;
using Skywalker.Extensions.RabbitMQ.Abstractions;
using Skywalker.Serialization.Abstractions;

namespace Skywalker.Extensions.RabbitMQ;

public class Utf8JsonRabbitMqSerializer : IRabbitMqSerializer
{
    private readonly ISerializer _jsonSerializer;

    public Utf8JsonRabbitMqSerializer(ISerializer jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
    }

    public byte[] Serialize(object obj)
    {
        return Encoding.UTF8.GetBytes(_jsonSerializer.Serialize(obj));
    }

    public object? Deserialize(byte[] value, Type type)
    {
        return _jsonSerializer.Deserialize(type, value);
    }

    public T? Deserialize<T>(byte[] value)
    {
        return _jsonSerializer.Deserialize<T>(value);
    }
}
