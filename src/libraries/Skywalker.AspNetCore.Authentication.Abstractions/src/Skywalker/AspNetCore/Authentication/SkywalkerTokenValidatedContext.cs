using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Skywalker.AspNetCore.Authentication
{
    public class SkywalkerTokenValidatedContext : ResultContext<SkywalkerAuthenticationOptions>
    {
        public SkywalkerTokenValidatedContext(HttpContext context, AuthenticationScheme scheme, SkywalkerAuthenticationOptions options)
            : base(context, scheme, options)
        {
        }

    }
}
