using DotNetty.Transport.Channels;
using Skywalker.Lightning.Messaging;
using Skywalker.Lightning.Terminal.Abstractions;

namespace Skywalker.Lightning
{
    public class TerminalMessageHandler : ChannelHandlerAdapter
    {
        private ILightningTerminalFactory? _terminalFactory { get; }

        public TerminalMessageHandler(ILightningTerminalFactory terminalFactory)
        {
            _terminalFactory = terminalFactory;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var msg = message as LightningMessage<LightningResponse>;
            var listener = context.Channel.GetAttribute(_terminalFactory!.MessageListenerAttributeKey).Get();
            listener.Received(msg!);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            //Logger.LogCritical("The status of Terminal {0} is unavailable,Please check the network and certificate!", context.Channel.RemoteAddress);
            //var ctx = context.Channel.GetAttribute(TransportContextAttributeKey).Get();
            //TerminalFactory.RemoveTerminal(ctx.Host, ctx.Port).GetAwaiter().GetResult();
            base.ChannelInactive(context);
        }
    }
}
