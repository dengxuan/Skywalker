using Skywalker.Lightning.Terminal.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Skywalker.Lightning
{
    internal class ClusterNodeContainer : IClusterNodeContainer
    {
        private readonly HashSet<IPEndPoint> _endPoints = new HashSet<IPEndPoint>();

        public void Add(IPEndPoint endPoint)
        {
            lock (_endPoints)
            {
                _endPoints.Add(endPoint);
            }
        }

        public IPEndPoint Get()
        {
            lock (_endPoints)
            {
                return _endPoints.FirstOrDefault()!;
            }
        }

        public void Remove(IPEndPoint endPoint)
        {
            lock (_endPoints)
            {
                _endPoints.Remove(endPoint);
            }
        }
    }
}
