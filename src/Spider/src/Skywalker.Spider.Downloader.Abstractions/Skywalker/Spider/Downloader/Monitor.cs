using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Skywalker.Spider.Downloader.Abstractions;
using Skywalker.Spider.Http;
using Skywalker.Spider.Messaging;
using Skywalker.Spider.Messaging.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Spider.Downloader
{
    public class Monitor : BackgroundService
    {

        private readonly ILogger<Monitor> _logger;


        private readonly IMessager _messager;


        private readonly IDownloader _downloader;

        private readonly IList<MessageConsumer<byte[]>> _consumers;

        private readonly DownloaderOptions _options;

        public Monitor(ILogger<Monitor> logger, IMessager messager, IDownloader downloader, IList<MessageConsumer<byte[]>> consumers, DownloaderOptions options)
        {
            _logger = logger;
            _messager = messager;
            _downloader = downloader;
            _consumers = consumers;
            _options = options;
        }

        private async Task HandleMessageAsync(byte[] bytes)
        {
            object? message;
            try
            {
                message = JsonSerializer.Deserialize<object>(new ReadOnlySpan<byte>(bytes));
                if (message == null)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Deserialize message failed: {e}");
                return;
            }

            switch (message)
            {
                case Request request:
                    Task.Factory.StartNew(async () =>
                    {
                        var response = await _downloader.DownloadAsync(request);
                        if (response == null)
                        {
                            return;
                        }

                        response.Agent = _options.Id;

                        var topic = $"Downloader-{request.Owner}";
                        await _messager.PublishAsync(topic, JsonSerializer.SerializeToUtf8Bytes(response));

                        _logger.LogInformation($"Downloader {_options.Name} download {request.RequestUri}, {request.Hash} for {request.Owner} completed");
                    }).ConfigureAwait(false).GetAwaiter();
                    break;
                default:
                    {
                        var msg = JsonSerializer.Serialize(message);
                        _logger.LogWarning($"Message not supported: {msg}");
                        break;
                    }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Monitor: [{Id}][{Name}] is starting", _options.Id, _options.Name);
            var consumer = new MessageConsumer<byte[]>(_options.Id);
            consumer.Received += HandleMessageAsync;
            await _messager.ConsumeAsync(consumer, stoppingToken);
            _consumers.Add(consumer);
        }
    }
}
