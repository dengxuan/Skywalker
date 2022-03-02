using System.Threading.Tasks;

namespace Skywalker.Permissions;

public interface IPermissionsAuthorizationService
{
    Task CheckAsync(PermissionsAuthorizationContext context);
}
