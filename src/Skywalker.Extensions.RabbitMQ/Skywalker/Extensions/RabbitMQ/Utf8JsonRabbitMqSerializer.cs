using System.Text.Json;
using Skywalker.DependencyInjection;
using Skywalker.Extensions.RabbitMQ.Abstractions;

namespace Skywalker.Extensions.RabbitMQ;

public class Utf8JsonRabbitMqSerializer : IRabbitMqSerializer, ISingletonDependency
{
    public byte[] Serialize(object obj)
    {
        return JsonSerializer.SerializeToUtf8Bytes(obj);
    }

    public object? Deserialize(byte[] value, Type type)
    {
        return JsonSerializer.Deserialize(value, type);
    }

    public T? Deserialize<T>(byte[] value)
    {
        return JsonSerializer.Deserialize<T>(value);
    }
}
