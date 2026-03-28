using System.Text.Json;
using Microsoft.Extensions.Options;
using Skywalker.DependencyInjection;
using Skywalker.Extensions.RabbitMQ.Abstractions;

namespace Skywalker.Extensions.RabbitMQ;

public class Utf8JsonRabbitMqSerializer : IRabbitMqSerializer, ISingletonDependency
{
    private readonly JsonSerializerOptions _options;

    public Utf8JsonRabbitMqSerializer(IOptions<JsonSerializerOptions>? options = null)
    {
        _options = options?.Value ?? new JsonSerializerOptions();
    }

    public byte[] Serialize(object obj)
    {
        return JsonSerializer.SerializeToUtf8Bytes(obj, _options);
    }

    public object? Deserialize(byte[] value, Type type)
    {
        return JsonSerializer.Deserialize(value, type, _options);
    }

    public T? Deserialize<T>(byte[] value)
    {
        return JsonSerializer.Deserialize<T>(value, _options);
    }
}
