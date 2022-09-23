namespace Skywalker.Extensions.DependencyInjection.Interceptors;

/// <summary>
/// The default implementation of <see cref="InvocationContext"/>
/// </summary>   
public sealed class DefaultInvocationContext : InvocationContext
{
    public DefaultInvocationContext(IServiceProvider services, params object[] arguments) : base(services, arguments)
    {
    }
}
