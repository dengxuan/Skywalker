﻿using Skywalker.Caching.Abstractions;

namespace Skywalker.Caching;

public static class SerializerExtensions
{
    /// <summary>
    /// Deserializes the byte array into an instance of T
    /// </summary>
    /// <param name="serializer">Serializer <see cref="ICachingSerializer"/> </param>
    /// <param name="bytes">The data to deserialize</param>
    /// <typeparam name="T">The type to instantiate</typeparam>
    /// <returns>The deserialized object</returns>
    public static T? Deserialize<T>(this ICachingSerializer serializer, byte[]? bytes)
    {
        if (serializer == null)
        {
            throw new ArgumentNullException(nameof(serializer));
        }
        return (T?)serializer.Deserialize(typeof(T), bytes);
    }
}
