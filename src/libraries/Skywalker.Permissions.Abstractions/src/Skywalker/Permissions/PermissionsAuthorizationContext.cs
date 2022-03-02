using System.Reflection;

namespace Skywalker.Permissions;

public class PermissionsAuthorizationContext
{
    public MethodInfo Method { get; }

    public PermissionsAuthorizationContext(MethodInfo method)
    {
        Method = method;
    }
}
