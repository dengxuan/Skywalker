using Skywalker.Security.Claims;
using System.Security.Principal;

namespace Skywalker.Security.Clients
{
    public class CurrentClient : ICurrentClient
    {
        public virtual string? Id => _principalAccessor?.Principal?.FindClientId();

        public virtual bool IsAuthenticated => Id != null;

        private readonly ICurrentPrincipalAccessor _principalAccessor;

        public CurrentClient(ICurrentPrincipalAccessor principalAccessor)
        {
            _principalAccessor = principalAccessor;
        }
    }
}
