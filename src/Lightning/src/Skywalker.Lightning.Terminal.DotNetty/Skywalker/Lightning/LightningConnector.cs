using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Skywalker.Lightning.Messaging;
using Skywalker.Lightning.Terminal.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
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

        public async Task Connect(string ipAddress, int port)
        {
            var bootstrap = new Bootstrap();
            bootstrap
                .Group(_loopGroup)
                .Channel<TcpSocketChannel>()
                .RemoteAddress(IPAddress.Parse(ipAddress), port)
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
                    pipeline.AddLast(new IdleStateHandler(0, 10, 0));
                    pipeline.AddLast("echo", new LightningConnectorMessageHandler());
                }));
            IChannel channel = await bootstrap.ConnectAsync();
            await channel.WriteAndFlushAsync("123");
            Console.Read();
        }
    }
}
