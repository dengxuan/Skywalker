using Microsoft.Extensions.Logging;
using Skywalker.EventBus.Abstractions;
using Skywalker.Spider.Downloader.Abstractions;
using Skywalker.Spider.Http;
using System.Threading.Tasks;

namespace Skywalker.Spider.HttpDownloader
{
    public class HttpRequestHandler : IEventHandler<Request>
    {
        private readonly IDownloader _downloader;

        private readonly IEventBus _eventBus;

        private readonly ILogger<HttpRequestHandler> _logger;

        public HttpRequestHandler(IDownloader downloader, IEventBus eventBus, ILogger<HttpRequestHandler> logger)
        {
            _downloader = downloader;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task HandleEventAsync(Request request)
        {
            Response response = await _downloader.DownloadAsync(request);
            _logger.LogInformation(response.ToString());
            await _eventBus.PublishAsync(response);
        }
    }
}
