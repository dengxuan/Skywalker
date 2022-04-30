using System;
using System.Collections.Generic;
using System.Text;

namespace Skywalker.Ddd.Application.Pipeline;

public abstract class InvocationContext
{
    /// <summary>
    /// Gets the proxy object on which the intercepted method is invoked.
    /// </summary>
    public abstract object Proxy { get; }

    /// <summary>
    /// Gets the object on which the invocation is performed.
    /// </summary>
    public abstract object Target { get; }

    /// <summary>
    /// Gets the arguments that target method has been invoked with.
    /// </summary>
    /// <remarks>Each argument is writable.</remarks>
    public abstract object[] Arguments { get; }

    /// <summary>
    /// Gets or sets the return value of the method.
    /// </summary>
    public abstract object? ReturnValue { get; set; }

    /// <summary>
    /// Gets the extended properties.
    /// </summary>
    /// <value>
    /// The extended properties.
    /// </value>
    public abstract IDictionary<string, object> Properties { get; }
}
