using System.Reflection;

namespace Skywalker.Aspects
{
    /// <summary>
    /// Represents the invocation context specific to calling the proxy.
    /// </summary>
    public abstract class InvocationContext
    {
        public abstract IServiceProvider Services { get; }

        /// <summary>
        /// Gets the object on which the invocation is performed.
        /// </summary>
        internal abstract object Target { get; }

        internal abstract MethodInfo Method { get; }

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
}