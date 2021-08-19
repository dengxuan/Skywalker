using Microsoft.AspNetCore.Authentication;

namespace Skywalker.AspNetCore.Transfer
{
    public class SkywalkerAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Gets or sets the challenge to put in the "WWW-Authenticate" header.
        /// </summary>
        public string Challenge { get; set; } = SkywalkerAuthenticationDefaults.AuthenticationScheme;
    }
}
