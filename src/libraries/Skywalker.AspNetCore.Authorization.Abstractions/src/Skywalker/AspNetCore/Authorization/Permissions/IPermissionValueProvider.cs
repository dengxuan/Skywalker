using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.AspNetCore.Authorization.Permissions;

public interface IPermissionValueProvider
{
    string Name { get; }

    //TODO: Rename to GetResult? (CheckAsync throws exception by naming convention)
    Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context);

    Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context);
}
