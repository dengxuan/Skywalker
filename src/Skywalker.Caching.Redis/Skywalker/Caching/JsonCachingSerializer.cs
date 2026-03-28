using System.Text.Json;
using Microsoft.Extensions.Options;
using Skywalker.Caching.Abstractions;
using Skywalker.DependencyInjection;

namespace Skywalker.Caching;

internal class JsonCachingSerializer : ICachingSerializer, ISingletonDependency
{
    private readonly CachingOptions _options;

    public JsonCachingSerializer(IOptions<CachingOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc />
    public byte[] Serialize<T>(T @object)
    {
        if (@object == null)
        {
            throw new ArgumentNullException(nameof(@object));
        }

        var @string = JsonSerializer.Serialize(@object);
        return _options.Encoding.GetBytes(@string);
    }

    /// <inheritdoc />
    public object? Deserialize(Type type, byte[]? bytes)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        if (bytes == null)
        {
            return null;
        }

        var @string = _options.Encoding.GetString(bytes);
        return JsonSerializer.Deserialize(@string, type);
    }
}
