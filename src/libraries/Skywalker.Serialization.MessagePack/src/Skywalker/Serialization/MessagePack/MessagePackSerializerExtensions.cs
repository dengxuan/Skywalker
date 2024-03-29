﻿using MessagePack;

namespace Skywalker.Serialization.MessagePack;

public static class MessagePackSerializerExtensions
{
    private static readonly MessagePackSerializerOptions s_serializerOptions = MessagePackSerializer.Typeless.DefaultOptions.WithCompression(MessagePackCompression.Lz4Block);

    public static Task<byte[]> ToBytesAsync<T>(this T message) where T : notnull
    {
        return message.ToBytesAsync(typeof(T));
    }

    public static async Task<byte[]> ToBytesAsync(this object message, Type type)
    {
        message.NotNull(nameof(message));
        using var stream = new MemoryStream();
        await MessagePackSerializer.SerializeAsync(type, stream, message, s_serializerOptions);
        return stream.ToArray();
    }

    public static async Task<object?> FromBytesAsync(this byte[] bytes, CancellationToken cancellationToken = default)
    {
        return await bytes.FromBytesAsync<object>(cancellationToken);
    }

    public static async Task<object?> FromBytesAsync(this byte[] bytes, Type type, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(bytes);
        return await MessagePackSerializer.DeserializeAsync(type, stream, s_serializerOptions, cancellationToken);
    }

    public static async Task<T?> FromBytesAsync<T>(this byte[] bytes, CancellationToken cancellationToken = default) where T : class
    {
        var stream = new MemoryStream(bytes);
        var result = await MessagePackSerializer.DeserializeAsync(typeof(T), stream, s_serializerOptions, cancellationToken);
        return result as T;
    }

    public static async Task<object?> FromBytesAsync(this ReadOnlyMemory<byte> bytes, Type type, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(bytes.ToArray());
        return await MessagePackSerializer.DeserializeAsync(type, stream, s_serializerOptions, cancellationToken);
    }

    public static async Task<T?> FromBytesAsync<T>(this ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default) where T : class
    {
        var stream = new MemoryStream(bytes.ToArray());
        var result = await MessagePackSerializer.DeserializeAsync(typeof(T), stream, s_serializerOptions, cancellationToken);
        return result as T;
    }
}
