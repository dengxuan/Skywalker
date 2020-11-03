using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Skywalker.Lightning.Serializer;
using System.Collections.Generic;

namespace Skywalker.Lightning.Messaging
{
    public class LightningMessageDecoder<T> : MessageToMessageDecoder<IByteBuffer>
    {
        private readonly ILightningSerializer _serializer;


        public LightningMessageDecoder(ILightningSerializer codec)
        {
            _serializer = codec;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object?> output)
        {
            var len = message.ReadableBytes;
            var array = new byte[len];
            message.GetBytes(message.ReaderIndex, array, 0, len);
            output.Add(item: _serializer.Deserialize<LightningMessage<T>>(array));
        }
    }
}
