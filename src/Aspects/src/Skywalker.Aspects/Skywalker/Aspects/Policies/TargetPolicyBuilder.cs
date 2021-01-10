using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Skywalker.Aspects.Policies
{
    internal class TargetPolicyBuilder<T> : ITargetPolicyBuilder<T>
    {
        private readonly TargetTypePolicy _policy = new TargetTypePolicy(typeof(T));    
        public TargetTypePolicy Build() => _policy;
        public ITargetPolicyBuilder<T> IncludeProperty<TValue>(Expression<Func<T, TValue>> propertyAccessor, PropertyMethod propertyMethod)
        {
            PropertyInfo property = GetProperty(propertyAccessor);
            _policy.IncludedProperties[property.MetadataToken] = propertyMethod;
            return this;
        }
        public ITargetPolicyBuilder<T> ExcludeProperty<TValue>(Expression<Func<T, TValue>> propertyAccessor, PropertyMethod propertyMethod)
        {
            Check.NotNull(propertyAccessor, nameof(propertyAccessor));
            if (!(propertyAccessor.Body is MemberExpression expression))
            {
                throw new ArgumentException("The specified is not a property access expression.", nameof(propertyAccessor));
            }
            var property = expression.Member as PropertyInfo;
            if (null == property)
            {
                throw new ArgumentException("The specified is not a property access expression.", nameof(propertyAccessor));
            }
            _policy.ExcludedProperties[property.MetadataToken] = propertyMethod;
            return this;
        }  
        public ITargetPolicyBuilder<T> IncludeMethod(Expression<Action<T>> methodInvocation)
        {
            MethodCallExpression expression = GetMetehod(methodInvocation);
            _policy.IncludedMethods.Add(expression.Method.MetadataToken);
            return this;
        }       
        public ITargetPolicyBuilder<T> ExcludeMethod(Expression<Action<T>> methodInvocation)
        {
            Check.NotNull(methodInvocation, nameof(methodInvocation));
            if (!(methodInvocation.Body is MethodCallExpression expression))
            {
                throw new ArgumentException("The specified is not a method call expression.", nameof(methodInvocation));
            }
            _policy.ExludedMethods.Add(expression.Method.MetadataToken);
            return this;
        }    
        public ITargetPolicyBuilder<T> IncludeAllMembers()
        {
            _policy.IncludeAllMembers = true;
            return this;
        }
        private static MethodCallExpression GetMetehod(Expression<Action<T>> methodInvocation)
        {
            Check.NotNull(methodInvocation, nameof(methodInvocation));
            if (!(methodInvocation.Body is MethodCallExpression expression))
            {
                throw new ArgumentException("The specified is not a method call expression.", nameof(methodInvocation));
            }

            return expression;
        }   
        private static PropertyInfo GetProperty<TValue>(Expression<Func<T, TValue>> propertyAccessor)
        {
            Check.NotNull(propertyAccessor, nameof(propertyAccessor));
            if (!(propertyAccessor.Body is MemberExpression expression))
            {
                throw new ArgumentException("The specified is not a property access expression.", nameof(propertyAccessor));
            }
            var property = expression.Member as PropertyInfo;
            if (null == property)
            {
                throw new ArgumentException("The specified is not a property access expression.", nameof(propertyAccessor));
            }

            return property;
        }
    }
}
