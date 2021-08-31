using System;

namespace Skywalker.EventBus.MemoryChannels
{
    internal readonly struct Message
    {
        public string Id { get; }

        public ReadOnlyMemory<byte> Body {  get; }

        public Message(string id, ReadOnlyMemory<byte> body)
        {
            Id = id;
            Body = body;
        }
    }
}