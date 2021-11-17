using Skywalker.Domain.Repositories;
using Skywalker.Domain.Services;

namespace Skywalker.IdentityServer.Domain.DeviceAuthorizations
{
    public class DeviceAuthorizationManager : DomainService
    {
        private readonly IRepository<DeviceAuthorization> _deviceAuthorizations;

        public DeviceAuthorizationManager(IRepository<DeviceAuthorization> deviceAuthorizations)
        {
            _deviceAuthorizations = deviceAuthorizations;
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
            await _deviceAuthorizations.InsertAsync(new DeviceAuthorization(deviceCode, userCode, data));
        }

        /// <summary>
        /// Finds device authorization by user code.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        public async Task<DeviceCode?> FindByUserCodeAsync(string userCode)
        {
            DeviceAuthorization? deviceAuthorization = await _deviceAuthorizations.FindAsync(x => x.UserCode == userCode);

            return deviceAuthorization?.Data;
        }

        /// <summary>
        /// Finds device authorization by device code.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        public async Task<DeviceCode?> FindByDeviceCodeAsync(string deviceCode)
        {
            DeviceAuthorization? deviceAuthorization = await _deviceAuthorizations.FindAsync(x => x.DeviceCode == deviceCode);

            return deviceAuthorization?.Data;
        }

        /// <summary>
        /// Updates device authorization, searching by user code.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        /// <param name="data">The data.</param>
        public async Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            DeviceAuthorization? deviceAuthorization = await _deviceAuthorizations.FindAsync(x => x.UserCode == userCode);

            if (deviceAuthorization == null)
            {
                return;
            }
            deviceAuthorization.Data = data;
            await _deviceAuthorizations.UpdateAsync(deviceAuthorization);
        }

        /// <summary>
        /// Removes the device authorization, searching by device code.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        /// <returns></returns>
        public async Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            await _deviceAuthorizations.DeleteAsync(x => x.DeviceCode == deviceCode);
        }
    }
}
