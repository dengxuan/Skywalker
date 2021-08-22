using MessagePack;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker;

public static class MessagePackSerializerExtensions
{
    private static readonly MessagePackSerializerOptions _serializerOptions = MessagePackSerializer.Typeless.DefaultOptions.WithCompression(MessagePackCompression.Lz4Block);

    public static byte[] ToBytes(this object message)
    {
        message.NotNull(nameof(message));

        var bytes = MessagePackSerializer.Typeless.Serialize(message, _serializerOptions);
        return bytes;
    }

    public static async Task<object> FromBytesAsync(this byte[] bytes, CancellationToken cancellationToken = default)
    {
        var stream = new MemoryStream(bytes);
        return await MessagePackSerializer.Typeless.DeserializeAsync(stream, _serializerOptions, cancellationToken);
    }

    public static async Task<T?> FromBytesAsync<T>(this byte[] bytes, CancellationToken cancellationToken = default) where T : class
    {
        var result = await bytes.FromBytesAsync(cancellationToken);
        return result as T;
    }
}
