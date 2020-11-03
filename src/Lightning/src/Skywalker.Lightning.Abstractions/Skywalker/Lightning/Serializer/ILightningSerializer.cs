using Microsoft.Extensions.DependencyInjection;
using System;

namespace Skywalker.Lightning.Serializer
{
    public interface ILightningSerializer
    {
        byte[] Serialize(object message);

        byte[] Serialize<TMessage>(TMessage message);

        object? Deserialize(byte[] bytes);

        TMessage? Deserialize<TMessage>(byte[] bytes);
    }
}
