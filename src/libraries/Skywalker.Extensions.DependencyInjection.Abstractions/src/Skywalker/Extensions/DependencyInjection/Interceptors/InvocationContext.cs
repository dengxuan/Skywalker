namespace Skywalker.Extensions.DependencyInjection.Interceptors;

/// <summary>
/// Represents the invocation context specific to calling the proxy.
/// </summary>
public class InvocationContext
{
    public IServiceProvider Services { get; }

    /// <summary>
    /// Gets the arguments that target method has been invoked with.
    /// </summary>
    /// <remarks>Each argument is writable.</remarks>
    public object[] Arguments { get; }

    /// <summary>
    /// Gets or sets the return value of the method.
    /// </summary>
    public object ReturnValue { get; set; } = default!;

    /// <summary>
    /// Gets the extended properties.
    /// </summary>
    /// <value>
    /// The extended properties.
    /// </value>
    public IDictionary<string, object> Properties { get; }

    public InvocationContext(IServiceProvider services, object[] arguments)
    {
        Services = services;
        Arguments = arguments;
        Properties = new Dictionary<string, object>();
    }

}
