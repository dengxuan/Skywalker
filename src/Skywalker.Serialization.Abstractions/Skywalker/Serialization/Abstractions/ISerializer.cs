// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Serialization.Abstractions;

public interface ISerializer
{
    string Serialize(object @object, bool camelCase = true, bool indented = false);

    T? Deserialize<T>(byte[] bytes, bool camelCase = true);

    object? Deserialize(Type type, byte[] bytes, bool camelCase = true);
}
