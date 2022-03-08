using System.Reflection;

namespace Skywalker.Authorization.Permissions;

public class PermissionsAuthorizationContext
{
    public MethodInfo Method { get; }

    public PermissionsAuthorizationContext(MethodInfo method)
    {
        Method = method;
    }
}
