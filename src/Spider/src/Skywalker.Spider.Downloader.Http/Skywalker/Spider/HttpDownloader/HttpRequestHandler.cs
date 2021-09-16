using Skywalker.EventBus.Abstractions;
using Skywalker.Spider.Downloader.Abstractions;
using Skywalker.Spider.Http;
using System.Threading.Tasks;

namespace Skywalker.Spider.HttpDownloader
{
    public class HttpRequestHandler : IEventHandler<Request>
    {

        private readonly IEventBus _eventBus;

        private readonly IDownloader _downloader;

        public HttpRequestHandler(IDownloader downloader, IEventBus eventBus)
        {
            _eventBus = eventBus;
            _downloader = downloader;
        }

        public async Task HandleEventAsync(Request request)
        {
            Response response = await _downloader.DownloadAsync(request);
            await _eventBus.PublishAsync(response);
        }
    }
}
