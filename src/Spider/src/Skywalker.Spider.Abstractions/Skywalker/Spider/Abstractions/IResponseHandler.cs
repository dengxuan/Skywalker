using Skywalker.Spider.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Spider.Abstractions;

public interface IResponseHandler
{
    Task HandleAsync(Request request, Response response, CancellationToken cancellationToken);
}
