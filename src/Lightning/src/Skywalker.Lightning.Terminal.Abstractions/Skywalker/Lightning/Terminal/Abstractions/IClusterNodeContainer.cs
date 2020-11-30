using System.Net;
using System.Threading.Tasks;

namespace Skywalker.Lightning.Terminal.Abstractions
{
    internal interface IClusterNodeContainer
    {
        Task GetClusterNodeAsync();

        Task AddClusterNodeAsync(IPEndPoint endPoint);

        Task RemoveClusterNodeAsync(IPEndPoint endPoint);
    }
}
