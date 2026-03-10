// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Pipeline;

public sealed class PipelineContext
{
    public InterceptDelegate Intercept { get; }

    /// <summary>
    /// Gets the arguments that target method has been invoked with.
    /// </summary>
    /// <remarks>
    /// Each argument is writable.
    /// </remarks>
    public object[] Arguments { get; }

    /// <summary>
    /// Gets or sets the return value of the method.
    /// </summary>
    public object? ReturnValue { get; set; }

    /// <summary>
    /// Gets the extended properties.
    /// </summary>
    /// <value>
    /// The extended properties.
    /// </value>
    public IDictionary<string, object> Properties { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineContext"/> class.
    /// </summary>
    /// <param name="handler">The proxy object on which the intercepted method is invoked.</param>
    /// <param name="target">The object on which the invocation is performed.</param>
    /// <param name="arguments">The arguments that target method has been invoked with.</param>
    /// <exception cref="ArgumentNullException">The specified<paramref name="handler"/> is null.</exception>   
    /// <exception cref="ArgumentNullException">The specified<paramref name="target"/> is null.</exception>   
    /// <exception cref="ArgumentNullException">The specified<paramref name="arguments"/> is null.</exception>
    internal PipelineContext(InterceptDelegate intercept, params object[] arguments)
    {
        Intercept = intercept;
        Arguments = arguments;
        Properties = new Dictionary<string, object>();
    }
}
