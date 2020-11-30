using System.Collections.Generic;
using System.Linq;

namespace Skywalker.Lightning
{
    public class LightningDescriptor
    {
        private readonly HashSet<LightningAddress> _addresses = new HashSet<LightningAddress>();

        public IReadOnlyList<LightningAddress> Addresses
        {
            get
            {
                lock (_addresses)
                {
                    return _addresses.ToList();
                }
            }
        }

        public LightningDescriptor(LightningAddress address)
        {
            lock (_addresses)
            {
                _addresses.Add(address);
            }
        }

        public void AddLightingAddress(LightningAddress address)
        {
            lock (_addresses)
            {
                _addresses.Add(address);
            }
        }

        public void RemoveLightingAddress(string ipAddressWithPort)
        {
            lock (_addresses)
            {
                _addresses.RemoveWhere(match => match.Key == ipAddressWithPort);
            }
        }
    }
}
