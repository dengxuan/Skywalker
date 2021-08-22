using Skywalker.Spider.Http;
using System.Threading.Tasks;

namespace System.Net.Http;

public static class HttpResponseMessageExtensions
{
    public static async Task<Response> ToResponseAsync(this HttpResponseMessage httpResponseMessage)
    {
        var response = new Response { StatusCode = httpResponseMessage.StatusCode };

        foreach (var header in httpResponseMessage.Headers)
        {
            response.Headers.Add(header.Key, header.Value?.ToString());
        }

        response.Version = httpResponseMessage.Version == null
            ? HttpVersion.Version11
            : httpResponseMessage.Version;

        response.Headers.TransferEncodingChunked = httpResponseMessage.Headers.TransferEncodingChunked;

        byte[] bytes = await httpResponseMessage.Content.ReadAsByteArrayAsync();

        response.Content = new Skywalker.Spider.Http.ByteArrayContent(bytes);

        foreach (var header in httpResponseMessage.Content.Headers)
        {
            response.Content.Headers.Add(header.Key, header.Value?.ToString());
        }

        return response;
    }
}
