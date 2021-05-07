using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.AspNetCore.Transfer
{
    public class SkywalkerChallengeContext : PropertiesContext<SkywalkerAuthenticationOptions>
    {
        public SkywalkerChallengeContext(HttpContext context, AuthenticationScheme scheme, SkywalkerAuthenticationOptions options, AuthenticationProperties properties) : base(context, scheme, options, properties) { }

        /// <summary>
        /// Any failures encountered during the authentication process.
        /// </summary>
        public Exception? AuthenticateFailure { get; set; }

        /// <summary>
        /// If true, will skip any default logic for this challenge.
        /// </summary>
        public bool Handled { get; private set; }

        /// <summary>
        /// Skips any default logic for this challenge.
        /// </summary>
        public void HandleResponse() => Handled = true;
    }
}
