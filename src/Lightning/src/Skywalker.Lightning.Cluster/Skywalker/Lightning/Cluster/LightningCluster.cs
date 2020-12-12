using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Skywalker.Lightning.Messaging;
using Skywalker.Lightning.Serializer;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Lightning.Cluster
{
    public class LightningCluster : IHostedService
    {

        protected IEventLoopGroup Boss { get; }
        protected IEventLoopGroup Worker { get; }

        private readonly X509Certificate2? _x509Certificate2;
        private readonly ILightningSerializer _serializer;
        private readonly ILogger<LightningServerMessageHandler> _logger;
        private readonly ILightningDescriptorContainer _lightningDescriptorContainer;
        private readonly ILightningDescriptorResolver _descriptorResolver;

        public LightningCluster(ILightningDescriptorContainer lightningDescriptorContainer, ILightningDescriptorResolver descriptorResolver, ILightningSerializer serializer, ILogger<LightningServerMessageHandler> logger)
        {
            _lightningDescriptorContainer = lightningDescriptorContainer ?? throw new ArgumentNullException(nameof(lightningDescriptorContainer));
            _descriptorResolver = descriptorResolver;
            var dispatcher = new DispatcherEventLoopGroup();
            Boss = dispatcher;
            Worker = new WorkerEventLoopGroup(dispatcher);
            _serializer = serializer;
            _logger = logger;
            //_x509Certificate2 = new X509Certificate2("./socialnetwork.pfx", "123456");
        }

        public void DeregisterAsync(string name, LightningAddress address)
        {
            _lightningDescriptorContainer.RemoveLightningAddress(name, address.IPEndPoint);
            _logger.LogInformation($"Remove {address.IPEndPoint}");
        }

        public void RegisterAsync(string name, LightningAddress address)
        {
            _lightningDescriptorContainer.AddLightningAddress(name, address);
            _logger.LogInformation($"Add {address.IPEndPoint}");
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
                         pipeline.AddLast(new IdleStateHandler(10, 0, 0));
                         pipeline.AddLast("Lightning-msg", new LightningServerMessageHandler(_descriptorResolver, _logger));
                     }));

            IChannel boundChannel = await bootstrap.BindAsync(IPAddress.Any, 30000);
            cancellationToken.WaitHandle.WaitOne();
            await boundChannel.CloseAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(Boss.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)), Worker.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
        }
    }
}
