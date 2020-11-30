using System.Net;

namespace Skywalker.Lightning.Terminal.Abstractions
{
    internal interface IClusterNodeContainer
    {
        IPEndPoint Get();

        void Add(IPEndPoint endPoint);

        void Remove(IPEndPoint endPoint);
    }
}
