using System.Threading.Tasks;

namespace Skywalker.Authorization.Permissions;

public interface IPermissionsAuthorizationService
{
    Task CheckAsync(PermissionsAuthorizationContext context);
}
