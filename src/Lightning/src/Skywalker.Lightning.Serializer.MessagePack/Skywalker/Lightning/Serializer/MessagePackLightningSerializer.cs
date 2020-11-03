using MessagePack;
using System;
using System.Collections.Generic;

namespace Skywalker.Lightning.Serializer
{
    public class MessagePackLightningSerializer : ILightningSerializer
    {

        public object? Deserialize(byte[] bytes)
        {
            if (bytes.IsNullOrEmpty())
            {
                return null;
            }
            return MessagePackSerializer.Typeless.Deserialize(bytes);
        }

        public TMessage? Deserialize<TMessage>(byte[] bytes)
        {
            if (bytes.IsNullOrEmpty())
            {
                return default;
            }
            return (TMessage)MessagePackSerializer.Typeless.Deserialize(bytes);
        }

        public byte[] Serialize(object message)
        {
            return MessagePackSerializer.Typeless.Serialize(message);
        }

        public byte[] Serialize<TMessage>(TMessage message)
        {
            return MessagePackSerializer.Typeless.Serialize(message);
        }
    }
}
