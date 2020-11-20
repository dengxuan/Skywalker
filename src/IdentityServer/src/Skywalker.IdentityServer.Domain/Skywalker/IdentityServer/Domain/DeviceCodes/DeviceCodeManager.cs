using Skywalker.Domain.Repositories;
using Skywalker.Domain.Services;
using Skywalker.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.IdentityServer.Domain.DeviceCodes
{
    public class DeviceCodeManager : DomainService
    {
        private readonly IRepository<DeviceCode> _deviceCodes;

        public DeviceCodeManager(IRepository<DeviceCode> deviceCodes)
        {
            _deviceCodes = deviceCodes;
        }

        /// <summary>
        /// Stores the device authorization request.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        /// <param name="userCode">The user code.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public async Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data)
        {
           await _deviceCodes.InsertAsync(new DeviceAuthorization(deviceCode, userCode, data));
        }

        /// <summary>
        /// Finds device authorization by user code.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        public Task<DeviceCode> FindByUserCodeAsync(string userCode)
        {
            DeviceCode foundDeviceCode;

            lock (_repository)
            {
                foundDeviceCode = _repository.FirstOrDefault(x => x.UserCode == userCode)?.Data;
            }

            return Task.FromResult(foundDeviceCode);
        }

        /// <summary>
        /// Finds device authorization by device code.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        public Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            DeviceCode foundDeviceCode;

            lock (_repository)
            {
                foundDeviceCode = _repository.FirstOrDefault(x => x.DeviceCode == deviceCode)?.Data;
            }

            return Task.FromResult(foundDeviceCode);
        }

        /// <summary>
        /// Updates device authorization, searching by user code.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        /// <param name="data">The data.</param>
        public Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            lock (_repository)
            {
                var foundData = _repository.FirstOrDefault(x => x.UserCode == userCode);

                if (foundData != null)
                {
                    foundData.Data = data;
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes the device authorization, searching by device code.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        /// <returns></returns>
        public Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            lock (_repository)
            {
                var foundData = _repository.FirstOrDefault(x => x.DeviceCode == deviceCode);

                if (foundData != null)
                {
                    _repository.Remove(foundData);
                }
            }


            return Task.CompletedTask;
        }
    }
}
