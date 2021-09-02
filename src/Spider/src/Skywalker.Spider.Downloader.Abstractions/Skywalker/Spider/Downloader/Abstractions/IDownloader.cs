using Skywalker.Spider.Http;
using System.Threading.Tasks;

namespace Skywalker.Spider.Downloader.Abstractions;

public interface IDownloader
{
    Task<Response> DownloadAsync(Request request);
}
