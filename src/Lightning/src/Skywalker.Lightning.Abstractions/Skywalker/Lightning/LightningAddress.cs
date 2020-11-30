using System.Net;

namespace Skywalker.Lightning
{
    public struct LightningAddress
    {
        internal string Key => IPEndPoint.ToString();

        public IPEndPoint IPEndPoint { get; set; }

        public int Weight { get; set; }

        public bool IsTls { get; set; }

        public LightningAddress(IPEndPoint ipEndPoint, int weight, bool isTls)
        {
            IPEndPoint = ipEndPoint;
            Weight = weight;
            IsTls = isTls;
        }
    }
}
