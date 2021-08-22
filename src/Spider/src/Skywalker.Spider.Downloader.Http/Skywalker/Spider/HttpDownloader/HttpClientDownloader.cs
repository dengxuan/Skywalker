using Skywalker.Spider.Downloader;
using Skywalker.Spider.Downloader.Abstractions;
using Skywalker.Spider.Http;
using System;
using System.Threading.Tasks;

namespace Skywalker.Spider.HttpDownloader
{
    public class HttpClientDownloader : IDownloader
    {
        public DownloaderTypes DownloaderType => DownloaderTypes.Http;

        public Task<Response> DownloadAsync(Request request)
        {
            throw new NotImplementedException();
        }
    }
}
