using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Skywalker.Lightning.Serializer;

namespace Skywalker.Lightning.Messaging
{
    public class LightningMessageEncoder<T> : MessageToByteEncoder<LightningMessage<T>>
    {
        private readonly ILightningSerializer _serializer;

        public LightningMessageEncoder(ILightningSerializer codec)
        {
            _serializer = codec;
        }

        protected override void Encode(IChannelHandlerContext context, LightningMessage<T> message, IByteBuffer output)
        {
            output.WriteBytes(_serializer.Serialize(message));
        }
    }
}
