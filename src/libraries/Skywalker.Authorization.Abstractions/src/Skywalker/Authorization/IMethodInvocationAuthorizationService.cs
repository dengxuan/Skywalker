using System.Threading.Tasks;

namespace Skywalker.Authorization;

public interface IMethodInvocationAuthorizationService
{
    Task CheckAsync(MethodInvocationAuthorizationContext context);
}
