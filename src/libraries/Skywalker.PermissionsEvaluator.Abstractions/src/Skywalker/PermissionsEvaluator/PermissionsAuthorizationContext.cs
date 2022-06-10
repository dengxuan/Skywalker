using System.Reflection;

namespace Skywalker.PermissionsEvaluator;

public class PermissionsAuthorizationContext
{
    public MethodInfo Method { get; }

    public PermissionsAuthorizationContext(MethodInfo method)
    {
        Method = method;
    }
}
