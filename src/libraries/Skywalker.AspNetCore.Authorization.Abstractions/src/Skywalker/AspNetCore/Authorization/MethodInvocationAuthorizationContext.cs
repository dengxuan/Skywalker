using System.Reflection;

namespace Skywalker.Authorization;

public class MethodInvocationAuthorizationContext
{
    public MethodInfo Method { get; }

    public MethodInvocationAuthorizationContext(MethodInfo method)
    {
        Method = method;
    }
}
