using Microsoft.Extensions.Hosting;
using Skywalker.Messaging.Abstractions;
using Skywalker.Spider.Downloader.Abstractions;
using Skywalker.Spider.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Spider.HttpDownloader
{
    internal class HostedHttpDownloader : BackgroundService
    {
        private readonly IDownloader _downloader;
        private readonly IMessagePublisher _messagePublisher;

        private readonly IMessageSubscriber _messageSubscriber;

        public HostedHttpDownloader(IDownloader downloader, IMessagePublisher messagePublisher, IMessageSubscriber messageSubscriber)
        {
            _downloader = downloader;
            _messagePublisher = messagePublisher;
            _messageSubscriber = messageSubscriber;
        }

        protected async Task HandlerAsync(string routingKey, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken)
        {
            Request? request = await bytes.FromBytesAsync<Request>(cancellationToken);
            Response response = await _downloader.DownloadAsync(request!);
            byte[] responseBytes = await response.ToBytesAsync();
            await _messagePublisher.PublishAsync(request!.Owner, responseBytes, cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _messageSubscriber.SubscribeAsync("Http", HandlerAsync, stoppingToken);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
