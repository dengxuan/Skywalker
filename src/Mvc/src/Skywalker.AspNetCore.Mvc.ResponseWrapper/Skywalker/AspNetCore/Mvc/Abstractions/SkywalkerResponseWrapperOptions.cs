using Skywalker.AspNetCore.Mvc.Models;

namespace Skywalker.AspNetCore.Mvc
{
    public class SkywalkerResponseWrapperOptions
    {
        /// <summary>
        /// If this is set to true, all exception and details are sent directly to clients on an error.
        /// Default: false (Hermit hides exception details from clients except special exceptions.)
        /// </summary>
        public bool SendAllExceptionsToClients { get; set; } = false;

        public WrapResultAttribute? DefaultWrapResultAttribute { get; }

        /// <summary>
        /// Default: true.
        /// </summary>
        public bool IsValidationEnabledForControllers { get; set; } = true;

        /// <summary>
        /// Default: true.
        /// </summary>
        public bool SetNoCacheForAjaxResponses { get; set; }
    }
}
