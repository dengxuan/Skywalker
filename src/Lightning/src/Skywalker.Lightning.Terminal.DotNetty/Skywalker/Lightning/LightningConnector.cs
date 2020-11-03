using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Skywalker.Lightning.Terminal.Abstractions;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Skywalker.Lightning
{
    public class LightningConnector : ILightningConnector
    {
        private readonly MultithreadEventLoopGroup _loopGroup;

        private readonly X509Certificate2? _x509Certificate2;

        public LightningConnector()
        {
            _loopGroup = new MultithreadEventLoopGroup();
            _x509Certificate2 = null;
        }

        public async Task Connect(string host, int port)
        {
            var bootstrap = new Bootstrap();
            bootstrap
                .Group(_loopGroup)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;

                    if (_x509Certificate2 != null)
                    {
                        string targetHost = _x509Certificate2.GetNameInfo(X509NameType.DnsName, false);
                        pipeline.AddLast("tls", new TlsHandler(stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true), new ClientTlsSettings(targetHost)));
                    }
                    pipeline.AddLast(new LoggingHandler());
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                    pipeline.AddLast("echo", new LightningConnectorMessageHandler());
                }));
            IChannel channel = await bootstrap.ConnectAsync(host, port);
        }

        internal class LightningConnectorMessageHandler : ChannelHandlerAdapter
        {

            public override void ChannelRead(IChannelHandlerContext context, object message)
            {
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
}
