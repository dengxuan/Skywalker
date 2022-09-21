// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Application.Abstractions;

namespace Skywalker.Ddd.Application.Pipeline;

public sealed class PipelineContext
{
    public IApplicationHandler Handler { get; set; }

    /// <summary>
    /// Gets the object on which the invocation is performed.
    /// </summary>
    /// <remarks>For virtual method based interception, the <see cref="Handler"/> and <see cref="Target"/> are the same.</remarks>
    public InterceptDelegate Target { get; }

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
    public object ReturnValue { get; set; }

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
    public PipelineContext(IApplicationHandler handler, InterceptDelegate target, params object[] arguments)
    {
        Handler = handler;
        Target = Check.NotNull(target, nameof(target));
        Arguments = Check.NotNull(arguments, nameof(arguments));
        ReturnValue = default!;
        Properties = new Dictionary<string, object>();
    }
}
