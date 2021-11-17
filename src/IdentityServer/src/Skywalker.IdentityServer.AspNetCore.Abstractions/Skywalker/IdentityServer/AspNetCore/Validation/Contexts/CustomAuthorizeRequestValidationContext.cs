using Skywalker.IdentityServer.AspNetCore.Validation.Models;

namespace Skywalker.IdentityServer.AspNetCore.Validation.Contexts
{
    /// <summary>
    /// Context for custom authorize request validation.
    /// </summary>
    public class CustomAuthorizeRequestValidationContext
    {
        /// <summary>
        /// The result of custom validation. 
        /// </summary>
        public AuthorizeRequestValidationResult Result { get; set; }
    }
}
