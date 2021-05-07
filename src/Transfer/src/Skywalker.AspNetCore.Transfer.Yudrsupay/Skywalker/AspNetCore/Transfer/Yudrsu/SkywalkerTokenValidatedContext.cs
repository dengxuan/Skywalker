using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Skywalker.AspNetCore.Transfer
{
    public class SkywalkerTokenValidatedContext : ResultContext<SkywalkerAuthenticationOptions>
    {
        public SkywalkerTokenValidatedContext(HttpContext context, TransferScheme scheme, SkywalkerAuthenticationOptions options)
            : base(context, scheme, options)
        {
        }

    }
}
