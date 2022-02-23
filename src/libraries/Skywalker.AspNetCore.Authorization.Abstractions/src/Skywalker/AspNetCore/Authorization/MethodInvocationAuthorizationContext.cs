using System.Reflection;

namespace Skywalker.AspNetCore.Authorization;

public class MethodInvocationAuthorizationContext
{
    public MethodInfo Method { get; }

    public MethodInvocationAuthorizationContext(MethodInfo method)
    {
        Method = method;
    }
}
