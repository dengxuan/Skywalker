using Microsoft.Extensions.DependencyInjection;
using Skywalker.Aspects.Abstractinons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Skywalker.Aspects
{
    /// <summary>
    /// The default implementation of interceptor chain builder.
    /// </summary>
    public class InterceptorChainBuilder : IInterceptorChainBuilder
    {
        private readonly List<Tuple<int, InterceptorDelegate>> _interceptors;
        private static readonly MethodInfo? _getServiceMethod = typeof(IServiceProvider).GetMethod("GetService", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly Dictionary<Type, InvokeDelegate> _invokers = new Dictionary<Type, InvokeDelegate>();
        private static readonly object _syncHelper = new object();

        /// <summary>
        /// Gets the service provider to get dependency services.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Create a new <see cref="InterceptorChainBuilder"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider to get dependency services.</param>
        /// <exception cref="ArgumentNullException">The argument <paramref name="serviceProvider"/> is null.</exception>
        public InterceptorChainBuilder(/*IServiceProvider serviceProvider*/)
        {
            //Check.NotNull(serviceProvider, nameof(serviceProvider));
            //Console.WriteLine("InterceptorChainBuilder:{0}", serviceProvider.GetHashCode());
            //ServiceProvider = serviceProvider;
            _interceptors = new List<Tuple<int, InterceptorDelegate>>();
        }

        private static bool TryGetInvoke(Type interceptorType, out InvokeDelegate? invoker)
        {
            if (_invokers.TryGetValue(interceptorType, out invoker))
            {
                return true;
            }

            lock (_syncHelper)
            {
                if (_invokers.TryGetValue(interceptorType, out invoker))
                {
                    return true;
                }

                var search = from it in interceptorType.GetTypeInfo().GetMethods()
                             let parameters = it.GetParameters()
                             where it.Name == "InvokeAsync" && it.ReturnType == typeof(Task) && parameters.FirstOrDefault()?.ParameterType == typeof(InvocationContext)
                             select it;
                MethodInfo? invokeAsyncMethod = search.FirstOrDefault();
                if (null == invokeAsyncMethod)
                {
                    return false;
                }

                ParameterExpression interceptor = Expression.Parameter(typeof(object), "interceptor");
                ParameterExpression invocationContext = Expression.Parameter(typeof(InvocationContext), "invocationContext");
                ParameterExpression serviceProvider = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

                var arguments = invokeAsyncMethod.GetParameters().Select(it => GetArgument(invocationContext, it.ParameterType));
                Expression instance = Expression.Convert(interceptor, interceptorType);
                var invoke = Expression.Call(instance, invokeAsyncMethod, arguments);
                invoker = Expression.Lambda<InvokeDelegate>(invoke, interceptor, invocationContext, serviceProvider).Compile();
                _invokers[interceptorType] = invoker;
            }
            return true;
        }

        private static Expression GetArgument(Expression invocationContext, Type parameterType)
        {
            if (parameterType == typeof(InvocationContext))
            {
                return invocationContext;
            }
            Expression serviceType = Expression.Constant(parameterType, typeof(Type));
            Expression callGetService = Expression.Call(_getServiceMethod!, serviceType);
            return Expression.Convert(callGetService, parameterType);
        }

        /// <summary>
        /// Register specified interceptor.
        /// </summary>
        /// <param name="interceptor">The interceptor to register.</param>
        /// <param name="order">The order for the registered interceptor in the interceptor chain.</param>
        /// <returns>The interceptor chain builder with registered intercetor.</returns>
        /// <exception cref="ArgumentNullException">The argument <paramref name="interceptor"/> is null.</exception>
        public IInterceptorChainBuilder Use(InterceptorDelegate interceptor, int order)
        {
            Check.NotNull(interceptor, nameof(interceptor));
            _interceptors.Add(new Tuple<int, InterceptorDelegate>(order, interceptor));
            return this;
        }

        /// <summary>
        /// Register specified interceptor.
        /// </summary>
        /// <param name="interceptorType">The type of interceptor to register.</param>
        /// <param name="order">The order for the registered interceptor in the interceptor chain.</param>
        /// <returns>The interceptor chain builder with registered intercetor.</returns>
        /// <exception cref="ArgumentNullException">The argument <paramref name="interceptor"/> is null.</exception>
        public IInterceptorChainBuilder Use(Type interceptorType, int order)
        {
            InterceptDelegate Intercept(InterceptDelegate next)
            {
                return async context =>
                {
                    context.Next = next;
                    if (TryGetInvoke(interceptorType, out var invoker))
                    {
                        object interceptor = ServiceProvider.GetRequiredService(interceptorType);
                        await invoker!(interceptor, context, ServiceProvider);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid interceptor type", "interceptorType");
                    }
                };
            }
            return Use(Intercept, order);
        }

        /// <summary>
        /// Build an interceptor chain using the registerd interceptors.
        /// </summary>
        /// <returns>A composite interceptor representing the interceptor chain.</returns>
        public InterceptorDelegate Build()
        {
            if (_interceptors.Count == 0)
            {
                return next => (_ => Task.CompletedTask);
            }

            if (_interceptors.Count == 1)
            {
                return _interceptors.Single().Item2;
            }

            var interceptors = _interceptors
                 .OrderBy(it => it.Item1)
                 .Select(it => it.Item2)
                 .Reverse()
                 .ToArray();

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
