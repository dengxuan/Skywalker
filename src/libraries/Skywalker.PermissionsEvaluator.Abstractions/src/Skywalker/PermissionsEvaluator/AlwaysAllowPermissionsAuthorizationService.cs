using System.Threading.Tasks;

namespace Skywalker.PermissionsEvaluator;

public class AlwaysAllowPermissionsAuthorizationService : IPermissionsAuthorizationService
{
    public Task CheckAsync(PermissionsAuthorizationContext context)
    {
        return Task.CompletedTask;
    }
}
