using System.Threading.Tasks;

namespace Skywalker.AspNetCore.Authorization;

public interface IMethodInvocationAuthorizationService
{
    Task CheckAsync(MethodInvocationAuthorizationContext context);
}
