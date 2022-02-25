using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Skywalker.Caching.Abstractions;

namespace Skywalker.Caching;

internal class JsonCachingSerializer : ICachingSerializer
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

        var @string = JsonConvert.SerializeObject(@object);
        return _options.Encoding.GetBytes(@string);
    }

    /// <inheritdoc />
    public object? Deserialize(Type type, byte[] bytes)
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
        return JsonConvert.DeserializeObject(@string, type);
    }
}
