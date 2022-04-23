using System;
using System.Collections.Generic;
using System.Linq;

/* Unmerged change from project 'Skywalker.AspNetCore.Authentication.Abstractions(netstandard2.1)'
Before:
namespace Skywalker.AspNetCore.Authentication
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
After:
namespace Skywalker.AspNetCore.Authentication;

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
*/

/* Unmerged change from project 'Skywalker.AspNetCore.Authentication.Abstractions(netcoreapp3.1)'
Before:
namespace Skywalker.AspNetCore.Authentication
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
After:
namespace Skywalker.AspNetCore.Authentication;

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
*/

/* Unmerged change from project 'Skywalker.AspNetCore.Authentication.Abstractions(net5.0)'
Before:
namespace Skywalker.AspNetCore.Authentication
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
After:
namespace Skywalker.AspNetCore.Authentication;

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
*/

/* Unmerged change from project 'Skywalker.AspNetCore.Authentication.Abstractions(net6.0)'
Before:
namespace Skywalker.AspNetCore.Authentication
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
After:
namespace Skywalker.AspNetCore.Authentication;

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
*/
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Skywalker.AspNetCore.Authentication;

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
