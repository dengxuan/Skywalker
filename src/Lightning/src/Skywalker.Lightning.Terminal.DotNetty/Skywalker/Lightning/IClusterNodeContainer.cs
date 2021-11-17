using System.Net;

namespace Skywalker.Lightning
{
    internal interface IClusterNodeContainer
    {
        IPEndPoint Get();

        void Add(IPEndPoint endPoint);

        void Remove(IPEndPoint endPoint);
    }
}
