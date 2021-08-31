using Skywalker.Spider.Downloader;
using Skywalker.Spider.Downloader.Abstractions;
using Skywalker.Spider.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace Skywalker.Spider.HttpDownloader;

public class HttpClientDownloader : IDownloader
{
    public DownloaderTypes DownloaderType => DownloaderTypes.Http;

    public async Task<Response> DownloadAsync(Request request)
    {
        HttpClient httpClient = new();
        HttpResponseMessage httpResponse = await httpClient.SendAsync(request.ToHttpRequestMessage());
        Response response = await httpResponse.ToResponseAsync();
        response.RequestHash = request.Hash;
        return response;
    }
}
