using Skywalker.Domain.Entities;
using System;

namespace Skywalker.IdentityServer.Domain.DeviceAuthorizations
{
    public class DeviceAuthorization : AggregateRoot<Guid>
    {

        public string DeviceCode { get; }

        public string UserCode { get; }

        public DeviceCode Data { get; set; }

        public DeviceAuthorization(string deviceCode, string userCode, DeviceCode data)
        {
            DeviceCode = deviceCode;
            UserCode = userCode;
            Data = data;
        }
    }
}
