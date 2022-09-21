using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Aspects.Abstractinons;
using System.Linq.Expressions;
using System.Reflection;

namespace Skywalker.Aspects
{
    /// <summary>
    /// The default implementation of interceptor chain builder.
    /// </summary>
    public sealed class InterceptorChainBuilder : IInterceptorChainBuilder
    {
        private readonly record struct Interceptor(InvokeDelegate Invoker, object Instance);


        private delegate Task InvokeDelegate(object interceptor, InvocationContext context);

        private readonly ISet<InterceptorDelegate> _interceptors;

        private readonly Dictionary<Type, Interceptor> _invokers = new();

        private static readonly object _locker = new();

        /// <summary>
        /// Create a new <see cref="InterceptorChainBuilder"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider to get dependency services.</param>
        /// <exception cref="ArgumentNullException">The argument <paramref name="serviceProvider"/> is null.</exception>
        public InterceptorChainBuilder()
        {
            _interceptors = new SortedSet<InterceptorDelegate>();
        }

        private bool TryGetTnterceptor(Type interceptorType, IServiceProvider service, InterceptDelegate next, out Interceptor interceptor)
        {
            if (_invokers.TryGetValue(interceptorType, out interceptor))
            {
                return true;
            }

            lock (_locker)
            {
                if (_invokers.TryGetValue(interceptorType, out interceptor))
                {
                    return true;
                }

                var search = from it in interceptorType.GetTypeInfo().GetMethods()
                             let parameters = it.GetParameters()
                             where it.Name == "InvokeAsync" && it.ReturnType == typeof(Task) && parameters.FirstOrDefault()?.ParameterType == typeof(InvocationContext)
                             select it;
                MethodInfo? invokeAsync = search.FirstOrDefault();
                if (invokeAsync == null)
                {
                    return false;
                }

                ParameterExpression interceptorExpression = Expression.Parameter(typeof(object), "interceptor");
                ParameterExpression invocationContextExpression = Expression.Parameter(typeof(InvocationContext), "context");

                Expression expression = Expression.Convert(interceptorExpression, interceptorType);
                var callExpression = Expression.Call(expression, invokeAsync, invocationContextExpression);
                var invoker = Expression.Lambda<InvokeDelegate>(callExpression, interceptorExpression, invocationContextExpression).Compile();
                var instance = ActivatorUtilities.CreateInstance(service, interceptorType, new object[] { next });
                _invokers[interceptorType] = interceptor = new Interceptor(invoker, instance);
            }
            return true;
        }

        /// <summary>
        /// Register the interceptor of <paramref name="interceptorType"/> to specified interceptor chain builder.
        /// </summary>
        /// <param name="builder">The interceptor chain builder to which the interceptor is registered.</param>
        /// <param name="interceptorType">The interceptor type.</param>
        /// <param name="order">The order for the registered interceptor in the built chain.</param>
        /// <param name="arguments">The non-injected arguments passes to the constructor.</param>
        /// <returns>The interceptor chain builder with registered interceptor.</returns>
        /// <exception cref="ArgumentNullException">The argument <paramref name="builder"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The argument <paramref name="interceptorType"/> is null.</exception>
        private IInterceptorChainBuilder Use(Type interceptorType)
        {
            Check.NotNull(interceptorType, nameof(interceptorType));

            InterceptDelegate Intercept(InterceptDelegate next)
            {
                return context =>
                {
                    if (!TryGetTnterceptor(interceptorType, context.Services, next, out var interceptor))
                    {
                        throw new ArgumentException("Invalid interceptor type {interceptorType}", interceptorType.Name);
                    }
                    return interceptor.Invoker(interceptor.Instance, context);
                };
            }
            return Use(Intercept);
        }

        /// <summary>
        /// Register the interceptor of <typeparamref name="TInterceptor"/> type to specified interceptor chain builder.
        /// </summary>
        /// <typeparam name="TInterceptor">The interceptor type.</typeparam>
        /// <param name="builder">The interceptor chain builder to which the interceptor is registered.</param>
        /// <returns>The interceptor chain builder with registered interceptor.</returns>
        /// <exception cref="ArgumentNullException">The argument <paramref name="builder"/> is null.</exception>
        public IInterceptorChainBuilder Use<TInterceptor>()
        {
            return Use(typeof(TInterceptor));
        }


        /// <summary>
        /// Register specified interceptor.
        /// </summary>
        /// <param name="interceptor">The interceptor to register.</param>
        /// <param name="order">The order for the registered interceptor in the interceptor chain.</param>
        /// <returns>The interceptor chain builder with registered intercetor.</returns>
        /// <exception cref="ArgumentNullException">The argument <paramref name="interceptor"/> is null.</exception>
        public IInterceptorChainBuilder Use(InterceptorDelegate interceptor)
        {
            Check.NotNull(interceptor, nameof(interceptor));
            _interceptors.Add(interceptor);
            return this;
        }

        /// <summary>
        /// Build an interceptor chain using the registerd interceptors.
        /// </summary>
        /// <returns>A composite interceptor representing the interceptor chain.</returns>
        public InterceptorDelegate Build()
        {
            if (_interceptors.IsNullOrEmpty())
            {
                return next => context => next(context);
            }

            if (_interceptors.Count == 1)
            {
                return _interceptors.Single();
            }

            var interceptors = _interceptors.Reverse().ToArray();

            return next =>
            {
                var current = next;
                for (int index = 0; index < interceptors.Length; index++)
                {
                    current = interceptors[index](current);
                }
                return current;
            };
        }

        /// <summary>
        /// Create a new interceptor chain builder which owns the same service provider as the current one.
        /// </summary>
        /// <returns>The new interceptor to create.</returns>
        public IInterceptorChainBuilder New()
        {
            return new InterceptorChainBuilder();
        }
    }
}
