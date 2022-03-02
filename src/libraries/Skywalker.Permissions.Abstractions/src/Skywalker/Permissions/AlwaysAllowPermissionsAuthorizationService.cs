using System.Threading.Tasks;

namespace Skywalker.Permissions;

public class AlwaysAllowPermissionsAuthorizationService : IPermissionsAuthorizationService
{
    public Task CheckAsync(PermissionsAuthorizationContext context)
    {
        return Task.CompletedTask;
    }
}
