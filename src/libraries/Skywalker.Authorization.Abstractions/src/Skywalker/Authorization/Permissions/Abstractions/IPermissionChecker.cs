using System.Security.Claims;

namespace Skywalker.Authorization.Permissions.Abstractions;

public interface IPermissionChecker
{
    Task<bool> IsGrantedAsync(string name);

    Task<bool> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string name);

    Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names);

    Task<MultiplePermissionGrantResult> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string[] names);
}
