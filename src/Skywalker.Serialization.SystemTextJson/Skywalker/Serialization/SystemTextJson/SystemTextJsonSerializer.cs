using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Skywalker.DependencyInjection;
using Skywalker.Serialization.Abstractions;

namespace Skywalker.Serialization.SystemTextJson;

public class SystemTextJsonSerializer : ISerializer, ISingletonDependency
{
    private readonly ClockDateTimeConverter _dateTimeConverter;

    public SystemTextJsonSerializer(ClockDateTimeConverter dateTimeConverter)
    {
        _dateTimeConverter = dateTimeConverter;
    }

    public string Serialize(object @object, bool camelCase = true, bool indented = false)
    {
        return JsonSerializer.Serialize(@object, CreateSerializerOptions(camelCase, indented));
    }

    public T? Deserialize<T>(byte[] bytes, bool camelCase = true)
    {
        var jsonString = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<T>(jsonString, CreateSerializerOptions(camelCase));
    }

    public object? Deserialize(Type type, byte[] bytes, bool camelCase = true)
    {
        var jsonString = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize(jsonString, type, CreateSerializerOptions(camelCase));
    }

    private JsonSerializerOptions CreateSerializerOptions(bool camelCase = true, bool indented = false)
    {
        var options = new JsonSerializerOptions();

        options.Converters.Add(_dateTimeConverter);

        if (camelCase)
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.DictionaryKeyPolicy = null; // Preserve dictionary keys as-is
        }

        if (indented)
        {
            options.WriteIndented = true;
        }

        return options;
    }
}
