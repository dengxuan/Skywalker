using System.Threading.Tasks;

namespace Skywalker.PermissionsEvaluator;

public interface IPermissionsAuthorizationService
{
    Task CheckAsync(PermissionsAuthorizationContext context);
}
