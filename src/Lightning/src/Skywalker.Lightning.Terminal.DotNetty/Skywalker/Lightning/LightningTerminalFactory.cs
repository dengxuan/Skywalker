using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skywalker.Lightning.Terminal;
using Skywalker.Lightning.Terminal.Abstractions;
using Skywalker.Lightning.Cluster.Abstractions;
using Skywalker.Lightning.Messaging;
using Skywalker.Lightning.Serializer;
using System;
using System.Collections.Concurrent;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Net;

namespace Skywalker.Lightning
{
    public class LightningTerminalFactory : ILightningTerminalFactory
    {
        private readonly ConcurrentDictionary<string, Task<ILightningTerminal>> _lightningTerminals = new ConcurrentDictionary<string, Task<ILightningTerminal>>();

        public AttributeKey<IMessageListener> MessageListenerAttributeKey { get; } = AttributeKey<IMessageListener>.ValueOf(typeof(LightningTerminalFactory), nameof(IMessageListener));

        protected MultithreadEventLoopGroup LoopGroup { get; }

        private readonly X509Certificate2? _x509Certificate2;

        private readonly IServiceProvider _serviceProvider;

        public LightningTerminalFactory(IServiceProvider serviceProvider)
        {
            LoopGroup = new MultithreadEventLoopGroup();
            _serviceProvider = serviceProvider;
            _x509Certificate2 = null;
            //_x509Certificate2 = new X509Certificate2("./socialnetwork.pfx", "123456");
        }

        public async Task<ILightningTerminal> CreateTerminalAsync(ILightningCluster cluster)
        {
            ILightningTerminal LightningTerminal = await _lightningTerminals.GetOrAdd($"{cluster.Address}-{cluster.Port}", async key =>
            {
                ILightningSerializer serializer = _serviceProvider.GetRequiredService<ILightningSerializer>();
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(LoopGroup)
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
                        pipeline.AddLast(new LightningMessageEncoder<LightningRequest>(serializer));
                        pipeline.AddLast(new LightningMessageDecoder<LightningResponse>(serializer));

                        pipeline.AddLast("echo", new TerminalMessageHandler(this));
                    }));
                IChannel channel = await bootstrap.ConnectAsync(IPAddress.Parse(cluster.Address), cluster.Port);

                var listener = new MessageListener();
                channel.GetAttribute(MessageListenerAttributeKey).Set(listener);
                ILogger<LightningTerminal> logger = _serviceProvider.GetRequiredService<ILogger<LightningTerminal>>();
                return new LightningTerminal(channel, LoopGroup, listener, logger, $"{cluster.Address}:{cluster.Port}", serializer);
            });
            return LightningTerminal;
        }

        public async Task RemoveTerminal(string name)
        {
            if (!_lightningTerminals.TryRemove(name, out var Terminal))
            {
                return;
            }
            await Terminal.Result.DisconnectAsync();
        }

        public async Task RemoveAllTerminal()
        {
            foreach (var key in _lightningTerminals.Keys)
            {
                await RemoveTerminal(key);
            }
        }
    }
}
