﻿namespace Skywalker.Extensions.RabbitMQ.Abstractions;

public interface IRabbitMqSerializer
{
    byte[] Serialize(object obj);

    object? Deserialize(byte[] value, Type type);

    T? Deserialize<T>(byte[] value);
}
