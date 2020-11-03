using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Lightning.Server;
using Skywalker.Lightning.Messaging;
using Skywalker.Lightning.Serializer;
using Skywalker.Lightning.Server.Abstractions;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace Skywalker.Lightning
{
    public class LightningServer : ILightningServer
    {
        protected IEventLoopGroup Boss { get; }
        protected IEventLoopGroup Worker { get; }
        public LightningServerOptions ServerOptions { get; }

        private readonly X509Certificate2? _x509Certificate2;
        private readonly ILightningSerializer _serializer;
        private readonly ILightningServiceFactory _LightningServiceFactory;
        private readonly ILogger<ServerMessageHandler> _logger;

        public LightningServer(IOptions<LightningServerOptions> options, ILightningSerializer serializer, ILightningServiceFactory LightningServiceFactory, ILogger<ServerMessageHandler> logger)
        {
            var dispatcher = new DispatcherEventLoopGroup();
            Boss = dispatcher;
            Worker = new WorkerEventLoopGroup(dispatcher);
            ServerOptions = options.Value;
            _serializer = serializer;
            _LightningServiceFactory = LightningServiceFactory;
            _logger = logger;
            //_x509Certificate2 = new X509Certificate2("./socialnetwork.pfx", "123456");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var bootstrap = new ServerBootstrap();
            bootstrap.Group(Boss, Worker);
            bootstrap.Channel<TcpServerChannel>();
            bootstrap.Option(ChannelOption.SoBacklog, 100)
                     .Handler(new LoggingHandler("SRV-LSTN"))
                     .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                     {
                         IChannelPipeline pipeline = channel.Pipeline;
                         if (_x509Certificate2 != null)
                         {
                             pipeline.AddLast("tls", TlsHandler.Server(_x509Certificate2));
                         }
                         pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                         pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                         pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));
                         pipeline.AddLast(new LightningMessageEncoder<LightningResponse>(_serializer));
                         pipeline.AddLast(new LightningMessageDecoder<LightningRequest>(_serializer));
                         pipeline.AddLast("Lightning-msg", new ServerMessageHandler(_LightningServiceFactory, _logger));
                     }));

            IChannel boundChannel = await bootstrap.BindAsync(IPAddress.Parse(ServerOptions.Host), ServerOptions.Port);
            cancellationToken.WaitHandle.WaitOne();
            await boundChannel.CloseAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(Boss.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)), Worker.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
        }
    }
}
