using System.Reflection;

namespace Skywalker.Aspects
{
    /// <summary>
    /// The default implementation of <see cref="InvocationContext"/>
    /// </summary>   
    public sealed class DefaultInvocationContext : InvocationContext
    {
        public override IServiceProvider Services { get; }

        /// <summary>
        /// Gets the object on which the invocation is performed.
        /// </summary>
        /// <remarks>For virtual method based interception, the <see cref="Proxy"/> and <see cref="Target"/> are the same.</remarks>
        internal override object Target { get; }

        internal override MethodInfo Method { get; }

        /// <summary>
        /// Gets the arguments that target method has been invoked with.
        /// </summary>
        /// <remarks>
        /// Each argument is writable.
        /// </remarks>
        public override object[] Arguments { get; }

        /// <summary>
        /// Gets or sets the return value of the method.
        /// </summary>
        public override object? ReturnValue { get; set; }

        /// <summary>
        /// Gets the extended properties.
        /// </summary>
        /// <value>
        /// The extended properties.
        /// </value>
        public override IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultInvocationContext"/> class.
        /// </summary>
        /// <param name="proxy">The proxy object on which the intercepted method is invoked.</param>
        /// <param name="target">The object on which the invocation is performed.</param>
        /// <param name="arguments">The arguments that target method has been invoked with.</param>
        /// <exception cref="ArgumentNullException">The specified<paramref name="proxy"/> is null.</exception>   
        /// <exception cref="ArgumentNullException">The specified<paramref name="target"/> is null.</exception>   
        /// <exception cref="ArgumentNullException">The specified<paramref name="arguments"/> is null.</exception>
        public DefaultInvocationContext(IServiceProvider services, MethodInfo method, object target, params object[] arguments)
        {
            Services = services;
            Method = method;
            Target = Check.NotNull(target, nameof(target));
            Arguments = Check.NotNull(arguments, nameof(arguments));
            Properties = new Dictionary<string, object>();
        }
    }
}
